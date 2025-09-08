using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentValidatorFunction.Models
{
    public class People
    {
        public string Name { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public byte IsActive { get; set; }

        public int Id { get; set; }
    }

}
