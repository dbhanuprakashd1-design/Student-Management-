using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

string connectionString = "Server=localhost\\SQLEXPRESS;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True";


// ✅ GET ALL STUDENTS
app.MapGet("/students", () =>
{
    var list = new List<object>();

    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var cmd = new SqlCommand("SELECT * FROM Students", conn);
    var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        list.Add(new
        {
            id = reader["Id"],
            name = reader["Name"],
            email = reader["Email"],
            year = reader["Year"],
            branch = reader["Branch"],
            rollNo = reader["RollNo"],
            mobile = reader["Mobile"]
        });
    }

    return list;
});


// ✅ ADD STUDENT
app.MapPost("/students", (Student s) =>
{
    try
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var cmd = new SqlCommand(
            "INSERT INTO Students (Name, Email, Year, Branch, RollNo, Mobile) VALUES (@Name, @Email, @Year, @Branch, @RollNo, @Mobile)", conn);

        cmd.Parameters.AddWithValue("@Name", s.Name);
        cmd.Parameters.AddWithValue("@Email", s.Email);
        cmd.Parameters.AddWithValue("@Year", s.Year);
        cmd.Parameters.AddWithValue("@Branch", s.Branch);
        cmd.Parameters.AddWithValue("@RollNo", s.RollNo);
        cmd.Parameters.AddWithValue("@Mobile", s.Mobile);

        cmd.ExecuteNonQuery();

        return Results.Ok("Student Added");
    }
    catch
    {
        return Results.BadRequest("Email or RollNo already exists");
    }
});


// ✅ GET STUDENT BY ID
app.MapGet("/students/{id}", (int id) =>
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var cmd = new SqlCommand("SELECT * FROM Students WHERE Id=@Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);
    var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        return Results.Ok(new
        {
            id = reader["Id"],
            name = reader["Name"],
            email = reader["Email"],
            year = reader["Year"],
            branch = reader["Branch"],
            rollNo = reader["RollNo"],
            mobile = reader["Mobile"]
        });
    }

    return Results.NotFound("Student not found");
});


// ✅ DELETE STUDENT
app.MapDelete("/students/{id}", (int id) =>
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var cmd = new SqlCommand("DELETE FROM Students WHERE Id=@Id", conn);
    cmd.Parameters.AddWithValue("@Id", id);

    cmd.ExecuteNonQuery();

    return Results.Ok("Deleted");
});


// ✅ UPDATE STUDENT
app.MapPut("/students/{id}", (int id, Student s) =>
{
    using var conn = new SqlConnection(connectionString);
    conn.Open();

    var cmd = new SqlCommand(
        "UPDATE Students SET Name=@Name, Email=@Email, Year=@Year, Branch=@Branch, RollNo=@RollNo, Mobile=@Mobile WHERE Id=@Id", conn);

    cmd.Parameters.AddWithValue("@Id", id);
    cmd.Parameters.AddWithValue("@Name", s.Name);
    cmd.Parameters.AddWithValue("@Email", s.Email);
    cmd.Parameters.AddWithValue("@Year", s.Year);
    cmd.Parameters.AddWithValue("@Branch", s.Branch);
    cmd.Parameters.AddWithValue("@RollNo", s.RollNo);
    cmd.Parameters.AddWithValue("@Mobile", s.Mobile);

    cmd.ExecuteNonQuery();

    return Results.Ok("Updated");
});

app.Run();


// ✅ MODEL
public record Student(
    string Name,
    string Email,
    int Year,
    string Branch,
    string RollNo,
    string Mobile
);