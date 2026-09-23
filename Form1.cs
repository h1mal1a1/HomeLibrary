using System.ComponentModel;
using HomeLibrary.Models;
using HomeLibrary.Data;
using HomeLibrary.Configuration;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
namespace HomeLibrary;

public partial class Form1 : Form
{
    private readonly TextBox _searchTextBox = new();
    private readonly Button _searchButton = new();
    private readonly DataGridView _booksGrid = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _openButton = new();
    private readonly BookRepository _bookRepository = new(AppConfiguration.GetConnectionString());
    private readonly BindingList<Book> _books = new();
    public Form1()
    {
        InitializeComponent();
        Text = "Домашняя библиотека";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 1000;
        Height = 650;

        _searchTextBox.Left = 20;
        _searchTextBox.Top = 20;
        _searchTextBox.Width = 700;
        _searchTextBox.PlaceholderText = "Поиск по названию, автору или оглавлению";

        _searchButton.Text = "Поиск";
        _searchButton.Left = 740;
        _searchButton.Top = 20;
        _searchButton.Width = 100;

        Controls.Add(_searchTextBox);
        Controls.Add(_searchButton);

        _booksGrid.Left = 20;
        _booksGrid.Top = 60;
        _booksGrid.Width = 940;
        _booksGrid.Height = 480;

        _booksGrid.ReadOnly = true;
        _booksGrid.AllowUserToAddRows = false;
        _booksGrid.MultiSelect = false;
        _booksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _booksGrid.AutoGenerateColumns = false;

        _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Название",
            DataPropertyName = "Title",
            Width = 400
        });

        _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Автор",
            DataPropertyName = "Author",
            Width = 300
        });

        _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Год",
            DataPropertyName = "PublicationYear",
            Width = 150
        });
        _booksGrid.DataSource = _books;
        Controls.Add(_booksGrid);
        _addButton.Text = "Добавить";
        _addButton.Left = 20;
        _addButton.Top = 555;
        _addButton.Width = 120;

        _editButton.Text = "Изменить";
        _editButton.Left = 150;
        _editButton.Top = 555;
        _editButton.Width = 120;

        _deleteButton.Text = "Удалить";
        _deleteButton.Left = 280;
        _deleteButton.Top = 555;
        _deleteButton.Width = 120;

        _openButton.Text = "Открыть";
        _openButton.Left = 410;
        _openButton.Top = 555;
        _openButton.Width = 120;

        Controls.Add(_addButton);
        Controls.Add(_editButton);
        Controls.Add(_deleteButton);
        Controls.Add(_openButton);

        _addButton.Click += (sender, e) =>
        {
            using var form = new BookForm();

            if (form.ShowDialog(this) == DialogResult.OK && form.Book != null)
            {
                try
                {
                    _bookRepository.Insert(form.Book);
                    LoadBooks();
                }
                catch (SqlException ex)
                {
                    ShowDatabaseError(ex);
                }
            }
        };

        _editButton.Click += (sender, e) =>
        {
            if (_booksGrid.CurrentRow?.DataBoundItem is not Book selectedBook)
            {
                MessageBox.Show("Выберите книгу.");
                return;
            }

            using var form = new BookForm(selectedBook);

            if (form.ShowDialog(this) == DialogResult.OK && form.Book != null)
            {
                try
                {
                    _bookRepository.Update(form.Book);
                    LoadBooks();
                }
                catch (SqlException ex)
                {
                    ShowDatabaseError(ex);
                }
            }
        };

        _deleteButton.Click += (sender, e) =>
        {
            if (_booksGrid.CurrentRow?.DataBoundItem is not Book selectedBook)
            {
                MessageBox.Show("Выберите книгу.");
                return;
            }

            var result = MessageBox.Show(
                $"Удалить книгу \"{selectedBook.Title}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    _bookRepository.Delete(selectedBook.Id);
                    LoadBooks();
                }
                catch (SqlException ex)
                {
                    ShowDatabaseError(ex);
                }
            }
        };

        _openButton.Click += (sender, e) =>
        {
            if (_booksGrid.CurrentRow?.DataBoundItem is not Book selectedBook)
            {
                MessageBox.Show("Выберите книгу.");
                return;
            }

            using var form = new BookForm(selectedBook, true);
            form.ShowDialog(this);
        };

        _searchButton.Click += (sender, e) =>
        {
            try
            {
                _books.Clear();

                var searchText = _searchTextBox.Text.Trim();

                var books = string.IsNullOrWhiteSpace(searchText)
                    ? _bookRepository.GetAll()
                    : _bookRepository.Search(searchText);

                foreach (var book in books)
                {
                    _books.Add(book);
                }
            }
            catch (SqlException ex)
            {
                ShowDatabaseError(ex);
            }
        };

        Shown += Form1_Shown;
    }

    private void Form1_Shown(object? sender, EventArgs e)
    {
        LoadBooks();
    }

    private void ShowDatabaseError(SqlException ex)
    {
        Debug.WriteLine(ex);

        MessageBox.Show(
            "Не удалось подключиться к базе данных.\n" +
            "Убедитесь, что SQL Server запущен.",
            "Ошибка БД",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }

    private void LoadBooks()
    {
        try
        {
            _books.Clear();

            var books = _bookRepository.GetAll();

            foreach (var book in books)
            {
                _books.Add(book);
            }
        }
        catch (SqlException ex)
        {
            ShowDatabaseError(ex);
        }
    }
}
