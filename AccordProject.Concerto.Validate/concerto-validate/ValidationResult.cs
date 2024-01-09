using System;
namespace AccordProject.Concerto.Validate
{
	public class ValidationResult
	{
		public string Instance { get; set; }
		public string Id { get; set; }
        public Boolean IsValid { get; set; }
        public string ErrorMessage{ get; set; }
	}
}

