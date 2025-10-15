namespace ApiContracts.SubForum;

public class SubForumDto
{
    public required int Id { get; set; }
    
    public required string Name { get; set; }
    public required string Description { get; set; }
}