// Declare the variable as int
int age;
// Prompt user to enter age
Console.WriteLine("Enter your age: ");
// Convert the input to int and store in age variable
age = int.Parse(Console.ReadLine());
// If-Else statement to check the age
if(age > 18)
    Console.WriteLine("You're eligible to vote");
else
    Console.WriteLine("You're not eligible to vote");
