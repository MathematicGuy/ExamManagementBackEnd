"""
    What are DTOs?

        Imagine you're sending a package to a friend. You wouldn't just throw all your stuff into a random box, right?
        You'd carefully select the items your friend needs, put them in a suitable container, and label it for easy identification.

        DTOs are like those containers for your data.
        They are simple classes specifically designed to package data for sending it between different parts of your application or across a network.

    Why Use DTOs (Data Transfer Objects) ?

        Control Data Exposure:  You only want to share certain information with your friends, right? 
        Similarly,DTOs let you choose exactly which properties to send to the client.
        You can exclude sensitive data, simplify complex relationships, or even rename properties to match the client's expectations.

        Evolving Models: Imagine you remodel your house. You might add new rooms or change the layout.
        But you'll still send your friend the same package because it's what they need. DTOs work the same way. 
        You can change your internal data models without breaking client applications that depend on the API,
        as long as you keep the DTOs the same.

        Performance: If you're sending a big package across the country, you might remove unnecessary items to save on shipping costs.
        DTOs help you optimize data transfer by sending only what's needed, reducing payload size and improving performance.

        Security: You don't want anyone tampering with your package, right?
        DTOs can be designed to prevent unauthorized changes to data, especially when receiving information from a client (over-posting protection).
"""
// Domain Model (Entity) - Your "House"
public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; } // You might not want to expose this in all cases
    public string SocialSecurityNumber { get; set; } // Definitely don't expose this!
}

// DTO - Your "Package"
public class StudentDTO
{
    public int StudentId { get; set; }
    public string Name { get; set; }
}

