using FincialWebApp1.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinancialWebApp1.Models
{
    public class MainClass
    {

        public int Id { get; set; }
    
        [DisplayName(" من قام بالتعديل ")]
        public string? WhoEdit { get; set;}
        [DisplayName(" تأريخ التعديل ")]
        public DateTime? DateOfEdit { get; set;}

        [DisplayName("من قام بالحذف")]
        public string? WhoDelet { get; set; }

        [DisplayName("تأريخ الحذف")]
        public DateTime? DateOfDelet { get; set; }
        [DisplayName(" تمت الاضافة بواسطة ")]
        public string? CreatedBy { get; set; }
        [DisplayName(" تأريخ الاضافة  ")]
        public DateTime? DateCreated { get; set; }

        // موظف وحدة الدفع الالكتروني
        [DisplayName("  رقم الCT")]
        public string? CTnumber { get; set; }

        [DisplayName(" تاريخ ال CT ")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CTdate { get; set; }

        [DisplayName("تاريخ الترحيل")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? dateofTravel { get; set; }

        [DisplayName("المصرف/الفرع")]
        public string? BankName { get; set; }

        [DisplayName(" مبلغ الدفع")]
        public decimal? Amount { get; set; }

        [DisplayName("المبلغ المتبقي")]
        public decimal? LeftAmount { get; set; }
        // موظف قطع الصندوق

        [DisplayName("جهة التجهيز")]
        public string? PreparationAuthority { get; set; }

        [DisplayName(" تاريخ قطع")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime? DateOfAddingbyCutterEmployer { get; set; }

        [DisplayName("المستودع")]
        public string? DepotName { get; set; }

        [DisplayName("الايراد")]
        public decimal? Import { get; set; }

        [DisplayName("مطبوعات")]
        public decimal? Stamps { get; set; }

        [DisplayName("تحميلات ")]
        public decimal? TransportationLoads { get; set; }

        public bool IsDeleted { get; set; }
        [DisplayName("اسم البنك ")]
        public int? BankListId { get; set; }
        public BankList? BankList { get; set; }

        [DisplayName(" ملاحظات الدفع الالكتروني")]
        public string? A { get; set; }

        [DisplayName(" ملاحظات القطع")]
        public string? B { get; set; }

        [DisplayName(" ملاحظات التدقيق")]
        public string? C { get; set; }

        [DisplayName("تم التدقيق")]
        public bool Ischecked { get; set; }

        [DisplayName("رقم الصك ")]
        public string? cheackNumber { get; set; }

        [DisplayName("تأريخ الصك ")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? cheackDate { get; set; }

        [DisplayName("تأريخ التدقيق ")]
        public DateTime? checkDate { get; set;}

    }
}
