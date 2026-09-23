using HomeLibrary.Models;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using HomeLibrary.Storage;

namespace HomeLibrary;

public class BookForm : Form
{
    private readonly TextBox _titleTextBox = new();
    private readonly TextBox _authorTextBox = new();
    private readonly NumericUpDown _yearInput = new();
    private readonly TextBox _descriptionTextBox = new();
    private readonly Button _saveButton = new();
    private readonly Button _cancelButton = new();
    private readonly WebView2 _contentsEditor = new();
    public Book? Book { get; private set; }
    private readonly Book? _sourceBook;
    private readonly bool _readOnly;

    public BookForm(Book? book = null, bool readOnly = false)
    {
        _sourceBook = book;
        _readOnly = readOnly;
        Text = readOnly ? "Просмотр книги" : book == null ? "Добавление книги" : "Редактирование книги";
        StartPosition = FormStartPosition.CenterParent;
        Width = 700;
        Height = 750;

        var titleLabel = new Label
        {
            Text = "Название:",
            Left = 20,
            Top = 25,
            Width = 100
        };

        _titleTextBox.Left = 130;
        _titleTextBox.Top = 20;
        _titleTextBox.Width = 500;


        var authorLabel = new Label
        {
            Text = "Автор:",
            Left = 20,
            Top = 65,
            Width = 100
        };

        _authorTextBox.Left = 130;
        _authorTextBox.Top = 60;
        _authorTextBox.Width = 500;


        var yearLabel = new Label
        {
            Text = "Год издания:",
            Left = 20,
            Top = 105,
            Width = 100
        };

        _yearInput.Left = 130;
        _yearInput.Top = 100;
        _yearInput.Width = 120;
        _yearInput.Minimum = 1;
        _yearInput.Maximum = DateTime.Now.Year;


        var descriptionLabel = new Label
        {
            Text = "Описание:",
            Left = 20,
            Top = 145,
            Width = 100
        };
        var contentsLabel = new Label
        {
            Text = "Оглавление:",
            Left = 20,
            Top = 285,
            Width = 100
        };

        _contentsEditor.Left = 130;
        _contentsEditor.Top = 280;
        _contentsEditor.Width = 500;
        _contentsEditor.Height = 300;

        Controls.Add(contentsLabel);
        Controls.Add(_contentsEditor);

        _descriptionTextBox.Left = 130;
        _descriptionTextBox.Top = 140;
        _descriptionTextBox.Width = 500;
        _descriptionTextBox.Height = 120;
        _descriptionTextBox.Multiline = true;


        _saveButton.Text = "Сохранить";
        _saveButton.Left = 380;
        _saveButton.Top = 620;
        _saveButton.Width = 120;

        _cancelButton.Text = "Отмена";
        _cancelButton.Left = 510;
        _cancelButton.Top = 620;
        _cancelButton.Width = 120;


        Controls.Add(titleLabel);
        Controls.Add(_titleTextBox);

        Controls.Add(authorLabel);
        Controls.Add(_authorTextBox);

        Controls.Add(yearLabel);
        Controls.Add(_yearInput);

        Controls.Add(descriptionLabel);
        Controls.Add(_descriptionTextBox);

        Controls.Add(_saveButton);
        Controls.Add(_cancelButton);

        Shown += BookForm_Shown;

        _saveButton.Click += SaveButton_Click;

        _cancelButton.Click += (sender, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        if (book != null)
        {
            _titleTextBox.Text = book.Title;
            _authorTextBox.Text = book.Author;
            _yearInput.Value = book.PublicationYear;
            _descriptionTextBox.Text = book.Description ?? string.Empty;
        }

        if (readOnly)
        {
            _titleTextBox.ReadOnly = true;
            _authorTextBox.ReadOnly = true;
            _yearInput.Enabled = false;
            _descriptionTextBox.ReadOnly = true;

            _saveButton.Visible = false;

            _cancelButton.Text = "Закрыть";
        }
    }

    private async void BookForm_Shown(object? sender, EventArgs e)
    {
        await _contentsEditor.EnsureCoreWebView2Async();

        _contentsEditor.NavigationCompleted += ContentsEditor_NavigationCompleted;

        _contentsEditor.NavigateToString("""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">

            <style>
                html, body {
                    margin: 0;
                    padding: 0;
                    background: white;
                    color: black;
                    font-family: Arial, sans-serif;
                    font-size: 14px;
                }

                #editor {
                    min-height: 260px;
                    padding: 10px;
                    outline: none;
                    background: white;
                    color: black;
                }

                #editor:empty::before {
                    content: "Введите оглавление...";
                    color: #888;
                }
            </style>
        </head>

        <body>
            <div id="editor" contenteditable="true"></div>
        </body>
        </html>
        """);
    }

    private async void ContentsEditor_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        _contentsEditor.NavigationCompleted -= ContentsEditor_NavigationCompleted;

        if (_sourceBook != null &&
            !string.IsNullOrWhiteSpace(_sourceBook.ContentsXml))
        {
            var xml = XDocument.Parse(_sourceBook.ContentsXml);
            var html = xml.Root?.Value ?? string.Empty;

            var htmlJson = JsonSerializer.Serialize(html);

            await _contentsEditor.CoreWebView2.ExecuteScriptAsync(
                $"document.getElementById('editor').innerHTML = {htmlJson};"
            );
        }

        if (_readOnly)
        {
            await _contentsEditor.CoreWebView2.ExecuteScriptAsync(
                "document.getElementById('editor').contentEditable = 'false';"
            );
        }
    }
    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        var title = _titleTextBox.Text.Trim();
        var author = _authorTextBox.Text.Trim();
        var publicationYear = (int)_yearInput.Value;

        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Укажите название книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            _titleTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            MessageBox.Show("Укажите автора.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            _authorTextBox.Focus();
            return;
        }

        if (publicationYear <= 0)
        {
            MessageBox.Show("Укажите год издания.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            _yearInput.Focus();
            return;
        }

        var htmlJson = await _contentsEditor.CoreWebView2.ExecuteScriptAsync(
            "document.getElementById('editor').innerHTML"
        );

        var html = JsonSerializer.Deserialize<string>(htmlJson) ?? string.Empty;

        var contentsXml = new XDocument(
            new XElement("contents",
                new XCData(html)
            )
        );
        var contentsXmlString = contentsXml.ToString();

        var contentsFilePath = BookContentsFileStorage.Save(
            contentsXmlString,
            _sourceBook?.ContentsFilePath
        );
        Book = new Book
        {
            Id = _sourceBook?.Id ?? 0,
            Title = title,
            Author = author,
            PublicationYear = publicationYear,
            Description = _descriptionTextBox.Text.Trim(),
            ContentsXml = contentsXmlString,
            ContentsFilePath = contentsFilePath
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}