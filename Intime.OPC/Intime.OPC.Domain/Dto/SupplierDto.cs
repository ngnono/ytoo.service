using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;


namespace Intime.OPC.Domain.Dto
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }

        public List<BrandDto> Brands { get; set; }
    }
}


