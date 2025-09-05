using System;
using System.Collections.Generic;

namespace WorkshopAPI.EF;

public partial class People
{
    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte IsActive { get; set; }

    public int Id { get; set; }
}
