using System.Text.Json;

public class Property : Printable<Property> {
    public String[] Attributes { get; set; } 
    public String[] Modifiers { get; set; } 
    public String Name { get; set; }
    public String Type { get; set; }
    public String Getter { get; set; }
    public String Setter { get; set; }
    public bool? IsStatic => !Modifiers?.Contains("instance");
}