using System;
using System.Collections.Generic;
using System.Text;

namespace Inventarisation.Models
{
    public class ProgramClass
    {
        public int Id { get; set; }
        public ProgramCreator ProgramCreator { get; set; } = new ProgramCreator();
        public int ProgramCreatorId { get; set; }
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
    }
}
