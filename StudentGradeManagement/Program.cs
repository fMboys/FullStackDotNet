// ============================================================================
//  Student Grade Manager - a console application
//  Features: add students, assign grades per subject, calculate averages,
//            and display student records.
// ============================================================================

namespace StudentGradeManager;

// ---------------------------------------------------------------------------
//  MODEL: a single grade for one subject
// ---------------------------------------------------------------------------
public class Grade
{
    public string Subject { get; }
    public double Score { get; set; }

    public Grade(string subject, double score)
    {
        Subject = subject;
        Score = score;
    }
}

// ---------------------------------------------------------------------------
//  MODEL: a student and their grades
// ---------------------------------------------------------------------------
public class Student
{
    private readonly List<Grade> _grades = new();

    public string Id { get; }
    public string Name { get; }
    public IReadOnlyList<Grade> Grades => _grades;

    public Student(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Adds a grade. If the student already has a grade for that subject
    /// (case-insensitive), the score is updated instead. Returns true when an
    /// existing grade was updated, false when a new one was added.
    /// </summary>
    public bool SetGrade(string subject, double score)
    {
        var existing = _grades.FirstOrDefault(g =>
            g.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            existing.Score = score;
            return true;
        }

        _grades.Add(new Grade(subject, score));
        return false;
    }

    /// <summary>Average of all grades, or null if the student has none.</summary>
    public double? CalculateAverage() =>
        _grades.Count == 0 ? null : _grades.Average(g => g.Score);
}

// ---------------------------------------------------------------------------
//  SERVICE: business logic (no Console calls here, so it is easy to test)
// ---------------------------------------------------------------------------
public class StudentManager
{
    private readonly Dictionary<string, Student> _students =
        new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<Student> AllStudents =>
        _students.Values.OrderBy(s => s.Id, StringComparer.OrdinalIgnoreCase);

    public int Count => _students.Count;

    public bool TryAddStudent(string id, string name, out string error)
    {
        if (string.IsNullOrWhiteSpace(id))   { error = "ID cannot be empty.";   return false; }
        if (string.IsNullOrWhiteSpace(name)) { error = "Name cannot be empty."; return false; }

        id = id.Trim();
        if (_students.ContainsKey(id))
        {
            error = $"A student with ID '{id}' already exists.";
            return false;
        }

        _students[id] = new Student(id, name.Trim());
        error = string.Empty;
        return true;
    }

    public Student? FindById(string id) =>
        _students.TryGetValue(id.Trim(), out var student) ? student : null;
}

// ---------------------------------------------------------------------------
//  UI: console menu and input helpers
// ---------------------------------------------------------------------------
public static class Program
{
    private const double MinScore = 0;
    private const double MaxScore = 100;

    private static readonly StudentManager Manager = new();

    public static void Main()
    {
        SeedSampleData();

        bool running = true;
        while (running)
        {
            ShowMenu();
            string choice = Prompt("Choose an option").Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1": AddStudent();          break;
                case "2": AddGrade();            break;
                case "3": ShowAverage();         break;
                case "4": DisplayOneStudent();   break;
                case "5": DisplayAllStudents();  break;
                case "0": running = false;       break;
                default:  Warn("Invalid option. Please enter a number from the menu."); break;
            }

            if (running) Pause();
        }

        Console.WriteLine("Goodbye!");
    }

    // ----- Menu actions -----------------------------------------------------

    private static void AddStudent()
    {
        Header("Add New Student");
        string id   = Prompt("Student ID");
        string name = Prompt("Student name");

        if (Manager.TryAddStudent(id, name, out string error))
            Success($"Student '{name.Trim()}' (ID: {id.Trim()}) added.");
        else
            Warn(error);
    }

    private static void AddGrade()
    {
        Header("Assign Grade");
        var student = AskForStudent();
        if (student == null) return;

        string subject = Prompt("Subject");
        if (string.IsNullOrWhiteSpace(subject))
        {
            Warn("Subject cannot be empty.");
            return;
        }

        double? score = PromptScore($"Grade ({MinScore}-{MaxScore})");
        if (score == null) return;

        bool updated = student.SetGrade(subject.Trim(), score.Value);
        Success(updated
            ? $"Updated {subject.Trim()} grade for {student.Name} to {score:0.##}."
            : $"Added {subject.Trim()} grade of {score:0.##} for {student.Name}.");
    }

    private static void ShowAverage()
    {
        Header("Student Average");
        var student = AskForStudent();
        if (student == null) return;

        double? avg = student.CalculateAverage();
        if (avg == null)
            Warn($"{student.Name} has no grades yet.");
        else
            Console.WriteLine($"Average grade for {student.Name}: {avg:0.00} ({LetterGrade(avg.Value)})");
    }

    private static void DisplayOneStudent()
    {
        Header("Student Record");
        var student = AskForStudent();
        if (student != null) PrintStudent(student);
    }

    private static void DisplayAllStudents()
    {
        Header("All Student Records");

        if (Manager.Count == 0)
        {
            Warn("No students have been added yet.");
            return;
        }

        foreach (var student in Manager.AllStudents)
            PrintStudent(student);
    }

    // ----- Display helpers --------------------------------------------------

    private static void PrintStudent(Student student)
    {
        Console.WriteLine(new string('-', 44));
        Console.WriteLine($"ID:   {student.Id}");
        Console.WriteLine($"Name: {student.Name}");

        if (student.Grades.Count == 0)
        {
            Console.WriteLine("Grades: (none)");
        }
        else
        {
            Console.WriteLine("Grades:");
            foreach (var g in student.Grades)
                Console.WriteLine($"  {g.Subject,-20} {g.Score,6:0.##}");

            double avg = student.CalculateAverage()!.Value;
            Console.WriteLine($"Average: {avg:0.00} ({LetterGrade(avg)})");
        }
        Console.WriteLine(new string('-', 44));
    }

    private static string LetterGrade(double avg) => avg switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _     => "F"
    };

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("        STUDENT GRADE MANAGER");
        Console.WriteLine("==========================================");
        Console.WriteLine(" 1. Add new student");
        Console.WriteLine(" 2. Assign grade to a student");
        Console.WriteLine(" 3. Calculate a student's average");
        Console.WriteLine(" 4. Display one student's record");
        Console.WriteLine(" 5. Display all student records");
        Console.WriteLine(" 0. Exit");
        Console.WriteLine("------------------------------------------");
    }

    // ----- Input helpers ----------------------------------------------------

    private static string Prompt(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine() ?? string.Empty;
    }

    private static Student? AskForStudent()
    {
        if (Manager.Count == 0)
        {
            Warn("No students have been added yet.");
            return null;
        }

        string id = Prompt("Student ID");
        var student = Manager.FindById(id);
        if (student == null)
            Warn($"No student found with ID '{id.Trim()}'.");
        return student;
    }

    private static double? PromptScore(string label)
    {
        string input = Prompt(label);

        if (!double.TryParse(input, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double score))
        {
            Warn("Grade must be a number.");
            return null;
        }

        if (score < MinScore || score > MaxScore)
        {
            Warn($"Grade must be between {MinScore} and {MaxScore}.");
            return null;
        }

        return score;
    }

    private static void Header(string title)
    {
        Console.WriteLine($"--- {title} ---");
    }

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

    // Two sample students so the app has something to show on first run.
    private static void SeedSampleData()
    {
        Manager.TryAddStudent("S001", "Ayesha Khan", out _);
        Manager.FindById("S001")!.SetGrade("Math", 92);
        Manager.FindById("S001")!.SetGrade("Physics", 85);

        Manager.TryAddStudent("S002", "Omar Malik", out _);
    }
}