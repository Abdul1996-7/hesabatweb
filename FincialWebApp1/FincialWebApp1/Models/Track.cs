using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinancialWebApp1.Models
{
    public class Track
    {
        public int Id { get; set; }
        public int TheId { get; set; }
        public string? WhoEdit { get; set; }
        public DateTime? DateOfEdit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }

        public string? WhoDelet { get; set; }
        public DateTime? DateOfDelet { get; set; }

        // موظف وحدة الدفع الالكتروني
        [DisplayName("  رقم الCT")]
        public string? CTnumber { get; set; }

        [DisplayName("تاريخ")]
        public DateTime? CTdate { get; set; }

        [DisplayName("المصرف/الفرع")]
        public string? BankName { get; set; }

        [DisplayName("المبلغ")]
        public decimal? Amount { get; set; }

        // موظف قطع الصندوق

        [DisplayName("جهة التجهيز")]
        public string? PreparationAuthority { get; set; }

        [DisplayName(" تاريخ قطع")]
        public DateTime? DateOfAddingbyCutterEmployer { get; set; }

        [DisplayName("المستودع")]
        public string? DepotName { get; set; }

        [DisplayName("الايراد")]
        public decimal? Import { get; set; }

        [DisplayName("مطبوعات")]
        public decimal? Stamps { get; set; }

        [DisplayName("تحميلات ")]
        public decimal? TransportationLoads { get; set; }

        // موظف قطع المستودعات

        [DisplayName("رقم  الاستمارة")]
        public string? FormCuttingNumber { get; set; }

        [DisplayName("نوع المنتوج")]
        public string? ProductType { get; set; }

        [DisplayName("سعر المنتوج")]
        public string? ProductPrice { get; set; }

        [DisplayName("الكمية المقطوعة")]
        public string? CutAmount { get; set; }

        [DisplayName("سعر الكمية المقطوعة")]
        public string? CutAmountCost { get; set; }

        [DisplayName("تاريخ مستودع")]
        public DateTime? DateofDEpot { get; set; }

        public int BankListId { get; set; }

        public BankList BankList { get; set; }

        [DisplayName(" ملاحظات  الدفع الالكتلروني ")]
        public string? A { get; set; }
        [DisplayName("ملاحظات القطع ")]
        public string? B { get; set; }

        [DisplayName("رقم الصك ")]
        public string? cheackNumber { get; set; }

        [DisplayName("تأريخ الصك ")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? cheackDate { get; set; }

        [DisplayName("ملاحظات التدقيق ")]
        public string? C { get; set; }

        [DisplayName("تأريخ التدقيق ")]
        public DateTime? checkDate { get; set; }

        [DisplayName("تم التدقيق")]
        public bool Ischecked { get; set; }

    }
}
