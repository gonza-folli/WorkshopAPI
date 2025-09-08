﻿namespace ContentValidatorFunction.Models
{
    public class ValidationResult
    {
        public bool Valid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
