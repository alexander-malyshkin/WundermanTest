namespace Model;

public sealed class Link
{
    public string Rel { get; set; }    
    public string Href { get; set; }
    public string Action { get; set; }
    public string[] Types { get; set; }
}