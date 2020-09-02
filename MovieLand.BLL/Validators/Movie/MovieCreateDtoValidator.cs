using FluentValidation;
using Microsoft.AspNetCore.Http;
using MovieLand.BLL.Dtos.Movie;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MovieLand.BLL.Validators.Movie
{
    public class MovieCreateDtoValidator : AbstractValidator<MovieCreateDto>
    {
        public MovieCreateDtoValidator() {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.OriginalName).MaximumLength(100);
            RuleFor(x => (int)x.Duration).InclusiveBetween(0,short.MaxValue);
            RuleFor(x => (int)x.MinAge).InclusiveBetween(0,100);
            RuleFor(x => x.ReleaseYear).InclusiveBetween(1800,2030);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Poster).Must(HaveAValidExtension).WithMessage("This file extension is not allowed").When(x => x.Poster != null);
            RuleFor(x => x.Poster.Length).InclusiveBetween(0, 1024 * 1024).When(x => x.Poster != null);
        }


        private static readonly string[] VALID_EXTENSIONS = { ".PNG", ".JPG", ".JPEG" };

        private bool HaveAValidExtension(IFormFile fileToValidate) {
            var extensionToValidate = Path.GetExtension(fileToValidate?.FileName).ToUpperInvariant();
            return VALID_EXTENSIONS.Contains(extensionToValidate);
        }
    }
}
