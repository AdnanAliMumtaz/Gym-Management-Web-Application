using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TransactionFee
    {
        [Key]
        public int TransactionFeeID {  get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime? DatePaid { get; set; }


        // Foreign Key
        public int MemberID { get; set; }
        public Member Member { get; set; }

    }
}
