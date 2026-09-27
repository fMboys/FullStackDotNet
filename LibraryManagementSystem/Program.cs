// ============================================================================
//  Library Management System - a console application
//  Features: search for a book, borrow (check out) a book with a 3-book
//            limit per user, and return (check in) a book.
// ============================================================================

namespace LibrarySystem;

// ---------------------------------------------------------------------------
//  MODEL: a single book in the library
// ---------------------------------------------------------------------------
public class Book
{
    public string Title { get; }
    public bool IsCheckedOut { get; private set; }

    public Book(string title)
    {
        Title = title;
        IsCheckedOut = false;
    }

    public void CheckOut() => IsCheckedOut = true;
    public void CheckIn() => IsCheckedOut = false;
}

// ---------------------------------------------------------------------------
//  SERVICE: the library's collection and borrowing rules
//  (no Console calls here, so this is easy to test independently of the UI)
// ---------------------------------------------------------------------------
public class Library
{
    public const int MaxBooksBorrowed = 3;

    private readonly List<Book> _books = new();

    /// <summary>How many books are currently checked out.</summary>
    public int BorrowedCount => _books.Count(b => b.IsCheckedOut);

    public IReadOnlyList<Book> AllBooks => _books;

    public void AddBook(string title) => _books.Add(new Book(title));

    /// <summary>Case-insensitive title search. Returns null if not found.</summary>
    public Book? FindByTitle(string title) =>
        _books.FirstOrDefault(b => b.Title.Equals(title.Trim(), StringComparison.OrdinalIgnoreCase));

    public enum CheckOutResult { Success, NotFound, AlreadyCheckedOut, LimitReached }

    public CheckOutResult CheckOut(string title)
    {
        var book = FindByTitle(title);
        if (book == null) return CheckOutResult.NotFound;
        if (book.IsCheckedOut) return CheckOutResult.AlreadyCheckedOut;
        if (BorrowedCount >= MaxBooksBorrowed) return CheckOutResult.LimitReached;

        book.CheckOut();
        return CheckOutResult.Success;
    }

    public enum CheckInResult { Success, NotFound, NotCheckedOut }

    public CheckInResult CheckIn(string title)
    {
        var book = FindByTitle(title);
        if (book == null) return CheckInResult.NotFound;
        if (!book.IsCheckedOut) return CheckInResult.NotCheckedOut;

        book.CheckIn();
        return CheckInResult.Success;
    }
}

// ---------------------------------------------------------------------------
//  UI: console menu and input handling
// ---------------------------------------------------------------------------
public static class Program
{
    private static readonly Library MyLibrary = new();

    public static void Main()
    {
        SeedSampleBooks();

        bool running = true;
        while (running)
        {
            ShowMenu();
            string choice = Prompt("Choose an option").Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1": SearchBook();       break;
                case "2": BorrowBook();       break;
                case "3": ReturnBook();       break;
                case "4": DisplayAllBooks();  break;
                case "0": running = false;    break;
                default:  Warn("Invalid option. Please enter a number from the menu."); break;
            }

            if (running) Pause();
        }

        Console.WriteLine("Goodbye!");
    }

    // ----- Menu actions -----------------------------------------------------

    private static void SearchBook()
    {
        Header("Search for a Book");
        string title = Prompt("Enter a book title to search for");

        if (string.IsNullOrWhiteSpace(title))
        {
            Warn("You didn't enter a title.");
            return;
        }

        var book = MyLibrary.FindByTitle(title);
        if (book == null)
        {
            Warn($"\"{title.Trim()}\" is not in the collection.");
        }
        else
        {
            Success($"\"{book.Title}\" is available in the collection.");
            Console.WriteLine(book.IsCheckedOut
                ? "  Status: currently checked out."
                : "  Status: on the shelf (not checked out).");
        }
    }

    private static void BorrowBook()
    {
        Header("Borrow (Check Out) a Book");
        Console.WriteLine($"Books currently borrowed: {MyLibrary.BorrowedCount} / {Library.MaxBooksBorrowed}");
        string title = Prompt("Enter the title to check out");

        if (string.IsNullOrWhiteSpace(title))
        {
            Warn("You didn't enter a title.");
            return;
        }

        var result = MyLibrary.CheckOut(title);
        switch (result)
        {
            case Library.CheckOutResult.Success:
                Success($"\"{title.Trim()}\" has been checked out to you.");
                break;
            case Library.CheckOutResult.NotFound:
                Warn($"\"{title.Trim()}\" is not in the collection.");
                break;
            case Library.CheckOutResult.AlreadyCheckedOut:
                Warn($"\"{title.Trim()}\" is already checked out.");
                break;
            case Library.CheckOutResult.LimitReached:
                Warn($"You've reached the limit of {Library.MaxBooksBorrowed} borrowed books. " +
                     "Return a book before borrowing another.");
                break;
        }
    }

    private static void ReturnBook()
    {
        Header("Return (Check In) a Book");
        string title = Prompt("Enter the title to check in");

        if (string.IsNullOrWhiteSpace(title))
        {
            Warn("You didn't enter a title.");
            return;
        }

        var result = MyLibrary.CheckIn(title);
        switch (result)
        {
            case Library.CheckInResult.Success:
                Success($"\"{title.Trim()}\" has been checked in. Thanks for returning it!");
                break;
            case Library.CheckInResult.NotFound:
                Warn($"\"{title.Trim()}\" is not in the collection.");
                break;
            case Library.CheckInResult.NotCheckedOut:
                Warn($"\"{title.Trim()}\" isn't currently checked out.");
                break;
        }
    }

    private static void DisplayAllBooks()
    {
        Header("All Books in the Collection");
        Console.WriteLine($"Books currently borrowed: {MyLibrary.BorrowedCount} / {Library.MaxBooksBorrowed}");
        Console.WriteLine();

        foreach (var book in MyLibrary.AllBooks)
        {
            string status = book.IsCheckedOut ? "Checked out" : "Available";
            Console.WriteLine($"  {book.Title,-30} {status}");
        }
    }

    // ----- Menu / input helpers ---------------------------------------------

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("        LIBRARY MANAGEMENT SYSTEM");
        Console.WriteLine("==========================================");
        Console.WriteLine(" 1. Search for a book");
        Console.WriteLine(" 2. Borrow (check out) a book");
        Console.WriteLine(" 3. Return (check in) a book");
        Console.WriteLine(" 4. Display all books");
        Console.WriteLine(" 0. Exit");
        Console.WriteLine("------------------------------------------");
    }

    private static string Prompt(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private static void Header(string title) => Console.WriteLine($"--- {title} ---");

    private static void Success(string message) => WriteColored(message, ConsoleColor.Green);
    private static void Warn(string message)    => WriteColored(message, ConsoleColor.Yellow);

    private static void WriteColored(string message, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }

    // A handful of sample books so the app has something to search on first run.
    private static void SeedSampleBooks()
    {
        MyLibrary.AddBook("The Great Gatsby");
        MyLibrary.AddBook("To Kill a Mockingbird");
        MyLibrary.AddBook("1984");
        MyLibrary.AddBook("Pride and Prejudice");
        MyLibrary.AddBook("The Hobbit");
        MyLibrary.AddBook("Moby Dick");
        MyLibrary.AddBook("War and Peace");
        MyLibrary.AddBook("The Catcher in the Rye");
    }
}