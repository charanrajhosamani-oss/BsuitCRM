
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BSuit.Core.Entities
{
    public class _BASE2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public Guid Id { get; set; } = Guid.NewGuid();




        [Column(Order = 101)]
        public int CreatedBy { get; set; } = 0;

        [Column(Order = 102)]
        public int ModifiedBy { get; set; } = 0;

        [Column(Order = 103)]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Column(Order = 104)]
        [DataType(DataType.DateTime)]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;


        [Column(Order = 105)]
        public bool IsActive { get; set; } = true;
    }
}
