namespace Entities;

public class SubForum
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CreatorUserId { get; set; }

    public SubForum()
    {
        
    }
    
    public SubForum(string name, string description)
    {
        Name = name;
        Description = description;
    }
}