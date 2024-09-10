using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.ModelViews;

namespace MinimalApiCSharp.Domain.DTOs
{
    public class ValidationErrorDTO
    {
        public ValidationErrors validationDTO(VehicleDTO vehicleDTO)
        {
            var validation = new ValidationErrors();

            if(string.IsNullOrEmpty(vehicleDTO.Name))
                validation.Messages.Add("Provide a Name.");

            if(string.IsNullOrEmpty(vehicleDTO.Brand))
                validation.Messages.Add("Provide a Brand.");

            if(vehicleDTO.Year < 1900)
                validation.Messages.Add("Provide a Year >= than 1900");

            return validation;

        }
    }
}