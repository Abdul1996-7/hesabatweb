using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FincialWebApp1.Data;
using FinancialWebApp1.Models;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FincialWebApp1.Models;
using System.Data;

namespace FincialWebApp1.Controllers
{
    public class MainClassesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AdministratorController> logger;
        private readonly FincialWebApp1Context _context;
        private readonly ImainClass _ImainClass;

        public MainClassesController(FincialWebApp1Context context,
                                     RoleManager<IdentityRole> roleManager,
                                    UserManager<IdentityUser> userManager,
                                    ImainClass imainClass)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _ImainClass = imainClass;
        }


        [Authorize(Roles = "Super Admin ,Admin,Epay,Cutting,Checking")]

        public async Task<IActionResult> Index(int Id, DateTime? startDateCT, bool IsCheacked, DateTime? endDateCT, DateTime? startDate, DateTime? endDate, string SearchCTNum, string SearchBankName, string SearchPreparationAuthority, string SearchDepotName, string SearchFormCuttingNumber, string SearchProductType, string SearchCutAmount, DateTime? endDateCheack, DateTime? startDateCheack, int pg = 1)
            {
                    // Use LINQ to get a list of all active items in the 'MainClass' entity set.
               var searchData = _context.MainClass.OrderByDescending(mc => mc.CTdate)
                .Include(mc => mc.BankList)
               .Where(mc => !mc.IsDeleted);

            // Apply other search criteria as needed
            if (!string.IsNullOrEmpty(SearchCTNum))
            {
                searchData = searchData.Where(g => g.CTnumber.Contains(SearchCTNum));
            }

            if (!string.IsNullOrEmpty(SearchBankName))
            {
                searchData = searchData.Where(g => g.BankList.Name.Contains(SearchBankName));
            }

                    if (!string.IsNullOrEmpty(SearchPreparationAuthority))
            {
                searchData = searchData.Where(g => g.PreparationAuthority.Contains(SearchPreparationAuthority));
            }

            if (!string.IsNullOrEmpty(SearchDepotName))
            {
                searchData = searchData.Where(g => g.DepotName.Contains(SearchDepotName));
            }

            // If a start date for the search has been provided, filter the search data to only include items with a date on or after the start date.
            if (startDate != null)
            {
                searchData = searchData.Where(x => x.dateofTravel >= startDate.Value.Date);
            }

            // If an end date for the search has been provided, filter the search data to only include items with a date before the day after the end date (to include items on the end date).
            if (endDate != null)
            {
                var nextDay = endDate.Value.Date.AddDays(1);
                searchData = searchData.Where(e => e.dateofTravel < nextDay);
            }
            // If a start date for the search has been provided, filter the search data to only include items with a date on or after the start date.
            if (startDateCT != null)
            {
                searchData = searchData.Where(x => x.CTdate >= startDateCT.Value.Date);
            }

            // If an end date for the search has been provided, filter the search data to only include items with a date before the day after the end date (to include items on the end date).
            if (endDateCT != null)
            {
                var nextDay = endDateCT.Value.Date.AddDays(1);
                searchData = searchData.Where(e => e.CTdate < nextDay);
            }


            // If a start date for the search has been provided, filter the search data to only include items with a date on or after the start date.
            if (startDateCheack != null)
            {
                searchData = searchData.Where(x => x.checkDate>= startDateCheack.Value.Date);
            }

            // If an end date for the search has been provided, filter the search data to only include items with a date before the day after the end date (to include items on the end date).
            if (endDateCheack != null)
            {
                var nextDay = endDateCheack.Value.Date.AddDays(1);
                searchData = searchData.Where(e => e.checkDate < nextDay);
            }

            if (IsCheacked)
            {
                searchData = searchData.Where(e => e.Ischecked);
            }
         
            foreach (var item in searchData)
            {
                item.LeftAmount = item.Amount - (item.Import + item.TransportationLoads + item.Stamps);
            }
            // Save the changes back to the database
             await _context.SaveChangesAsync();
            // Set the page size to 100.
            const int pageSize = 100;

            // If the page number provided is less than 1, set it to 1.
            if (pg < 1)
                pg = 1;

            // Get the total count of items in the filtered search data.
            int rescCount = searchData.Count();

            // Create a new Pager object with the total count of items, current page number, and page size.
            var pager = new Pager(rescCount, pg, pageSize);

            // Calculate the number of items to skip based on the page number and page size.
            int recSkip = (pg - 1) * pageSize;

            // Get the data to display on the current page based on the number of items to skip and the page size.
            var data = searchData.Skip(recSkip).Take(pager.PageSize).ToList();

            // Set the ViewBag variable for the Pager object to use in the view.
            this.ViewBag.Pager = pager;

            //ViewBag.LeftAmount = Total;
            // Pass the search parameters to the ViewBag for use in the view.
            ViewBag.SearchCTNum = SearchCTNum;
            ViewBag.SearchBankName = SearchBankName;
            ViewBag.SearchPreparationAuthority = SearchPreparationAuthority;
            ViewBag.SearchDepotName = SearchDepotName;
            ViewBag.SearchFormCuttingNumber = SearchFormCuttingNumber;
            ViewBag.SearchProductType = SearchProductType;
            ViewBag.SearchCutAmount = SearchCutAmount;
            ViewBag.startDateCT = startDateCT;
            ViewBag.endDateCT = endDateCT;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.startDateCheack = startDateCheack;
            ViewBag.endDateCheack = endDateCheack;
            ViewBag.IsCheacked = IsCheacked;


            if (Request.Method == "POST")
            {
                var checkDate = DateTime.Parse(Request.Form["checkDate"]);
                // Create a new instance of YourModel and set properties
                var newEntry = new MainClass
                {
                    checkDate = checkDate,
                    // Set other properties
                };

                // Add the new entry to the context and save changes
                _context.MainClass.Add(newEntry);
                await _context.SaveChangesAsync();
            }


            return View( data);
        }
        [Authorize(Roles = "Super Admin ,Admin,Checking")]
        public async Task<IActionResult> Dashboard(DateTime? startDate,DateTime? endDate, DateTime? startDateEdit, DateTime? endDateEdit)
        {
            // Get the username of the current user
            string userName = HttpContext.User.Identity.Name;
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;
            DateTime enteredDate = DateTime.Now; // Replace with the entered date from the user
        


            // Conunt only the data that are entered by the Epay users
            if (User != null)
            {
                string roleName = "Epay";
                // Retrieve the users in the specified role
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                // Retrieve the CTs added by users with the "Epay" role today
                var ctList = await _context.MainClass
                    .Where(mc => mc.dateofTravel != null && mc.dateofTravel.HasValue && mc.dateofTravel.Value.Date == currentDate && !mc.IsDeleted && mc.Amount != null &&  mc.CTdate != null && mc.CTnumber != null) 
                    .ToListAsync();
                // Perform the join operation in memory
                int ctCount = ctList
                    .Join(usersInRole, mc => mc.WhoEdit, user => user.UserName, (mc, _) => mc)
                    .Count();
                ViewBag.CTCount = ctCount;
            }

          
           
            if (User != null)
            {
                // Retrieve the fully completed data counts for the entered date range
                var dataCountsTotal = new Dictionary<string, int>();

                string roleName = "Epay";

                // Retrieve the users in the specified role
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

                // Retrieve the CTs added by users with the "Epay" role
                var ctList = await _context.MainClass
                    .Where(mc => mc.CreatedBy != null && mc.CTdate.HasValue && !mc.IsDeleted)
                    .ToListAsync();

                // Perform the join operation in memory
                int ctCountTotal = ctList
                    .Join(usersInRole, mc => mc.CreatedBy, user => user.UserName, (mc, _) => mc)
                    .Count();


                dataCountsTotal.Add("البيانات", ctCountTotal);


                ViewBag.CTCountTotal = dataCountsTotal;
            }



            // Conunt only the data that are Edited by the Cutting users
            if (User != null)
            {
                string roleName = "Cutting";
                // Retrieve the users in the specified role
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                // Retrieve the CTs added by users with the "Epay" role today
                var editList = await _context.MainClass
                    .Where(mc => mc.DateOfAddingbyCutterEmployer != null && mc.DateOfAddingbyCutterEmployer.HasValue && mc.DateOfAddingbyCutterEmployer.Value.Date == currentDate && !mc.IsDeleted && mc.Stamps != null && mc.Import != null && mc.TransportationLoads != null && mc.DepotName != null)
                    .ToListAsync();
                // Perform the join operation in memory
                int editCTCount = editList
                    .Join(usersInRole, mc => mc.WhoEdit, user => user.UserName, (mc, _) => mc)
                    .Count();
                ViewBag.EditCount = editCTCount;
            }

            if (User != null)
            {
                // Retrieve the fully completed data counts for the entered date range
                var dataCountsTotalCutting = new Dictionary<string, int>();

                string roleName = "Cutting";

                // Retrieve the users in the specified role
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

                // Retrieve the CTs added by users with the "Epay" role
                var ctList = await _context.MainClass
                    .Where(mc => mc.WhoEdit != null && mc.DateOfEdit.HasValue && !mc.IsDeleted)
                    .ToListAsync();

                // Perform the join operation in memory
                int ctCountTotalCutting = ctList
                    .Join(usersInRole, mc => mc.WhoEdit, user => user.UserName, (mc, _) => mc)
                    .Count();


                dataCountsTotalCutting.Add("البيانات", ctCountTotalCutting);


                ViewBag.CTCountTotalCutting = dataCountsTotalCutting;
            }

            // count the number of data that has been fully completed
            var fullFiledData = _context.MainClass.Count(mc =>
            mc.dateofTravel != null &&
            !string.IsNullOrEmpty(mc.CTnumber) &&
            !string.IsNullOrEmpty(mc.BankName) &&
            (mc.Amount)!=null &&
            !string.IsNullOrEmpty(mc.PreparationAuthority) &&
            mc.DateOfAddingbyCutterEmployer != null &&
            !string.IsNullOrEmpty(mc.DepotName) &&
            (mc.Import) != null &&
            (mc.Stamps) != null &&
            (mc.TransportationLoads) != null &&
            mc.dateofTravel.HasValue && mc.dateofTravel.Value.Date == currentDate &&
            !mc.IsDeleted);


            int delteCTcount = await _context.MainClass.Where(mc => mc.IsDeleted == true &&
              mc.DateOfDelet.HasValue && mc.DateOfDelet.Value.Date == currentDate).CountAsync();



            // Retrieve the fully completed data counts for the entered date range
            var dataCounts = new Dictionary<string, int>();

            // Retrieve the not completed data counts for the entered date range
            var dataCountsNotcompleted = new Dictionary<string, int>();

            // Retrieve the fully completed data count for the entered date range
            int count = await _context.MainClass
                .Where(mc => mc.dateofTravel != null &&
                             !string.IsNullOrEmpty(mc.CTnumber) &&
                             !string.IsNullOrEmpty(mc.BankName) &&
                             (mc.Amount) != null &&
                             !string.IsNullOrEmpty(mc.PreparationAuthority) &&
                             mc.DateOfAddingbyCutterEmployer != null &&
                             !string.IsNullOrEmpty(mc.DepotName) &&
                             (mc.Import) != null &&
                             (mc.Stamps) != null &&
                             (mc.TransportationLoads) != null &&
                             mc.DateOfEdit.Value.Date >= DateTime.MinValue.Date &&
                             mc.DateOfEdit.Value.Date <= DateTime.Now.Date &&
                             !mc.IsDeleted)
                .CountAsync();

            dataCounts.Add("البيانات", count);

               int countNotcompleted = await _context.MainClass
              .Where(mc =>
                  ((mc.dateofTravel == null ||
                  string.IsNullOrEmpty(mc.CTnumber) ||
                  string.IsNullOrEmpty(mc.BankName) ||
                  (mc.Amount) != null) ||
                  string.IsNullOrEmpty(mc.PreparationAuthority) ||
                  mc.DateOfAddingbyCutterEmployer == null ||
                  string.IsNullOrEmpty(mc.DepotName) ||
                  (mc.Import) != null ||
                  (mc.Stamps) != null ||
                  (mc.TransportationLoads) != null) &&
                  (mc.dateofTravel.Value.Date >= DateTime.MinValue.Date &&
                  mc.dateofTravel.Value.Date <= DateTime.Now.Date) &&
                  !mc.IsDeleted)
              .CountAsync();


            dataCountsNotcompleted.Add("بيانات غير مكتملة ", countNotcompleted);

            ViewBag.DataCounts = dataCounts;
            ViewBag.DataCountsNotcompleted = dataCountsNotcompleted;

            // Pass the count to the view
            ViewBag.FullFiledData = fullFiledData;
            ViewBag.DelteCTcount = delteCTcount;
       
            return View();
        }


        // GET: MainClasses/ExportToExcel
        public IActionResult ExportToExcel()
        {
            var viewModel = new ExportToExcelViewModel();
            return View(viewModel);
        }

        [Authorize(Roles = "Super Admin ,Admin,Epay,Cutting,Checking")]

        [HttpPost]
        public IActionResult ExportToExcel(ExportToExcelViewModel viewModel)
        {
            // Set the license context for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Retrieve data from the database based on the date range
            var data = _context.MainClass
            .Where(mc => mc.CTdate.HasValue && mc.CTdate >= viewModel.StartDate && mc.CTdate <= viewModel.EndDate.AddDays(1) && !mc.IsDeleted)
            .ToList();

               
            // Create a new Excel package
            using (var package = new ExcelPackage())
                {
                    // Create the worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    // Set column headers
                    worksheet.Cells[1, 1].Value = "رقم الCT";
                    worksheet.Cells[1, 2].Value = "الCT تاريخ";
                    worksheet.Cells[1, 3].Value = "تاريخ الترحيل";
                    worksheet.Cells[1, 4].Value = "المصرف/الفرع";
                    worksheet.Cells[1, 5].Value = "مبلغ الدفع";
                    worksheet.Cells[1, 6].Value = "جهة التجهيز";
                    worksheet.Cells[1, 7].Value = "تاريخ قطع";
                    worksheet.Cells[1, 8].Value = "الايراد";
                    worksheet.Cells[1, 9].Value = "مطبوعات";
                    worksheet.Cells[1, 10].Value = "تحميلات";
                    worksheet.Cells[1, 11].Value = "المبلغ المتبقي";
                    worksheet.Cells[1, 12].Value = "ملاحظات الدفع الالكتروني";
                    worksheet.Cells[1, 13].Value = "اسم المستودع";
                    worksheet.Cells[1, 14].Value = "رقم الصك";
                    worksheet.Cells[1, 15].Value = "تأريخ الصك ";
                    worksheet.Cells[1, 16].Value = " ملاحظات القطع ";
                // Add more column headers as needed

                // Set the data rows
                int row = 2;
                    foreach (var item in data)
                    {
                        worksheet.Cells[row, 1].Value = item.CTnumber;
                        worksheet.Cells[row, 2].Value = item.CTdate;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-MM-dd"; // Format the cell as a date
                        worksheet.Cells[row, 3].Value = item.dateofTravel;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-MM-dd"; // Format the cell as a date
                        worksheet.Cells[row, 4].Value = item.BankName;
                        worksheet.Cells[row, 5].Value = item.Amount;
                        worksheet.Cells[row, 6].Value = item.PreparationAuthority;
                        worksheet.Cells[row, 7].Value = item.DateOfAddingbyCutterEmployer;
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "yyyy-MM-dd"; // Format the cell as a date
                        worksheet.Cells[row, 8].Value = item.Import;
                        worksheet.Cells[row, 9].Value = item.Stamps;
                        worksheet.Cells[row, 10].Value = item.TransportationLoads;
                        worksheet.Cells[row, 11].Value = item.LeftAmount;
                        worksheet.Cells[row, 12].Value = item.A;
                        worksheet.Cells[row, 13].Value = item.DepotName;
                        worksheet.Cells[row, 14].Value = item.cheackNumber;
                        worksheet.Cells[row, 15].Value = item.cheackDate;
                        worksheet.Cells[row, 15].Style.Numberformat.Format = "yyyy-MM-dd"; // Format the cell as a date
                        worksheet.Cells[row, 16].Value = item.B;

                    row++;
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Generate a unique file name based on the date range
                    string fileName = $"DataExport_{viewModel.StartDate.ToString("yyyyMMdd")}_{viewModel.EndDate.ToString("yyyyMMdd")}.xlsx";

                    // Create a memory stream to store the Excel package
                    using (var stream = new MemoryStream())
                    {
                        // Save the Excel package to the stream
                        package.SaveAs(stream);

                        // Set the position of the stream to the beginning
                        stream.Position = 0;

                        // Set the content type and file name for the response
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileDownloadName = fileName;

                        // Return the Excel file as a file download response
                        return File(stream.ToArray(), contentType, fileDownloadName);
                    }
                }
            

            // If the model state is not valid, return the view with the same view model
            return View(viewModel);
        }
        //Impot Excell files
        public IActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ImportExcel(IFormFile fromFiles)
        {
            string path = _ImainClass.Documentupload(fromFiles);
            DataTable dt = _ImainClass.CustomerDataTable(path);
            _ImainClass.ImportCustomer(dt);



            return View();
        }


        [Authorize(Roles = "Super Admin,Epay")]
        // GET: MainClasses/Create
        public IActionResult Create(MainClass mainClass,int T)
        {
            
            // Retrieve the bank names from the database
            var bankList = _context.BankList.ToList();
            bankList.Insert(0, new BankList { BankListId = -1, Name = "Select a bank" });

            ViewData["BankListId"] = new SelectList(bankList, "BankListId", "Name");
            mainClass.CTnumber = string.Empty;
            return View();
        }

        // POST: MainClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin,Epay")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedBy,DateCreated,WhoEdit,DateOfEdit,WhoDelet,DateOfDelet,CTnumber,CTdate,BankNameId,Amount,PreparationAuthority,DateOfAddingbyCutterEmployer,DepotName,Import,Stamps,TransportationLoads,FormCuttingNumber,ProductType,ProductPrice,CutAmount,CutAmountCost,DateofDEpot,BankListId,Name,Branch,BankList,A,B,dateofTravel,cheackNumber,cheackDate,checkDate")] MainClass mainClass)
        {
            if (ModelState.IsValid)
            {

                string userName = HttpContext.User.Identity.Name;
                mainClass.CreatedBy = userName;
                mainClass.DateCreated = DateTime.Now;
                
               
            
                // Check if CTnumber already exists
                var existingMainClass = await _context.MainClass.FirstOrDefaultAsync(a => a.CTnumber == mainClass.CTnumber && a.IsDeleted == false);
                if (existingMainClass != null)
                {
                    // CTnumber already exists, retrieve related data
                    var relatedData = await _context.MainClass
                    .Where(mc => mc.CTnumber == mainClass.CTnumber && mc.IsDeleted == false)
                    .ToListAsync();

                    // Pass the related data to the view
                    ViewData["RelatedData"] = relatedData;

                    ModelState.AddModelError("CTnumber", "   تم ادخالة مسبقارقم ال(CT)  ");

                    // Retrieve the bank names from the database

                    return View(mainClass);
                }
                // Check if BankListId is not selected
                if (mainClass.BankListId == -1)
                {
                    ModelState.AddModelError("BankListId", "رجاءا قم بأختيار البنك");

                    // Retrieve the bank names from the database
                    var bankListt = _context.BankList.ToList();
                    bankListt.Insert(0, new BankList { BankListId = -1, Name = "اختار البنك" });
                    ViewData["BankListId"] = new SelectList(bankListt, "BankListId", "Name", mainClass.BankListId);

                    // Return the view with validation errors
                    return View(mainClass);
                }

                // BankListId is selected, proceed with creating the record
                _context.Add(mainClass);
                await _context.SaveChangesAsync();
                TempData["sucess"] = "تم الاضافة بنجاح";
                return RedirectToAction(nameof(Create));



            }

            // Model state is not valid, retrieve the bank names from the database
            var bankList = _context.BankList.ToList();
            bankList.Insert(0, new BankList { BankListId = -1, Name = "اختار البنك" });
            ViewData["BankListId"] = new SelectList(bankList, "BankListId", "Name", mainClass.BankListId);

            return View(mainClass);
        }

        [Authorize(Roles = "Super Admin ,Admin,Epay,Cutting,Checking")]
        // GET: MainClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainClass = await _context.MainClass.Include(m => m.BankList).FirstOrDefaultAsync(m => m.Id == id);
            if (mainClass == null)
            {
                return NotFound();
            }

            var bankList = await _context.BankList.ToListAsync();
            ViewData["BankListId"] = new SelectList(bankList, "BankListId", "Name", mainClass.BankListId);

            return View(mainClass);
        }
        // POST: MainClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        
        [Authorize(Roles = "Super Admin ,Admin,Epay,Cutting,Checking")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WhoEdit,CreatedBy,DateCreated,DateOfEdit,WhoDelet,DateOfDelet,CTnumber,CTdate,BankName,Amount,PreparationAuthority,DateOfAddingbyCutterEmployer,DepotName,Import,Stamps,TransportationLoads,FormCuttingNumber,ProductType,ProductPrice,CutAmount,CutAmountCost,DateofDEpot,BankListId,BankList,Name,A,B,dateofTravel,cheackNumber,cheackDate,checkDate,C")] MainClass mainClass)
        {
            if (ModelState.IsValid)
            {
                if (mainClass.Id == 0)
                {
                    _context.Update(mainClass);
                }
                else
                {
                    var existingMainClass = await _context.MainClass.Include(m => m.BankList).FirstOrDefaultAsync(m => m.Id == id);

                    if (existingMainClass != null)
                    {
                        // Check if CTnumber is changed
                        if (existingMainClass.CTnumber != mainClass.CTnumber)
                        {
                            // CTnumber has been changed, check if it already exists
                            var relatedData = await _context.MainClass
                                .Where(mc => mc.CTnumber == mainClass.CTnumber && !string.IsNullOrEmpty(mc.CTnumber))
                                .ToListAsync();

                            if (relatedData.Any())
                            {
                                // Pass the related data to the view
                                ViewData["RelatedData"] = relatedData;

                                ModelState.AddModelError("CTnumber", "   تم ادخالة مسبقارقم ال(CT)  ");

                                // Retrieve the bank names from the database

                                return View(mainClass);
                            }
                        }
                        var track = new Track
                        {
                            TheId = existingMainClass.Id,
                            CTnumber = existingMainClass.CTnumber,
                            CTdate = existingMainClass.CTdate,
                            cheackNumber = existingMainClass.cheackNumber,
                            cheackDate = existingMainClass.cheackDate,
                            BankListId = (int)mainClass.BankListId,
                            BankName = existingMainClass?.BankList?.Name,
                            Amount = existingMainClass?.Amount,
                            PreparationAuthority = existingMainClass?.PreparationAuthority,
                            DateOfAddingbyCutterEmployer = existingMainClass?.DateOfAddingbyCutterEmployer,
                            DepotName = existingMainClass?.DepotName,
                            Import = existingMainClass?.Import,
                            Stamps = existingMainClass?.Stamps,
                            TransportationLoads = existingMainClass?.TransportationLoads,
                            WhoEdit = HttpContext.User?.Identity?.Name,
                            DateOfEdit = DateTime.Now,
                            WhoDelet = existingMainClass?.WhoDelet,
                            DateOfDelet = existingMainClass?.DateOfDelet,
                            CreatedBy = existingMainClass?.CreatedBy,
                            A = existingMainClass?.A,
                            B = existingMainClass?.B,
                            C = existingMainClass?.C,
                            checkDate = existingMainClass?.checkDate,
                            Ischecked = existingMainClass.Ischecked,
                            DateCreated = DateTime.Now

                            
                        };
                        // Add the tracker to the context
                        _context.AddRange(track);

                        string userName1 = HttpContext.User.Identity.Name;
                        mainClass.WhoEdit = userName1;
                        mainClass.DateOfEdit = DateTime.Now;


                        // Get the logged-in user
                        var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                        if (currentUser != null)
                        {
                            // Check if the user has the "Epay" role
                            bool isInRole = await _userManager.IsInRoleAsync(currentUser, "Epay");
                            if (isInRole)
                            {
                                mainClass.dateofTravel = DateTime.Now;
                            }
                        }

                        var date = _context.MainClass.Find(id);
                        existingMainClass.WhoEdit = mainClass.WhoEdit;
                        existingMainClass.CreatedBy = mainClass.CreatedBy;
                        existingMainClass.DateCreated = mainClass.DateCreated;
                        existingMainClass.DateOfEdit = mainClass.DateOfEdit;
                        existingMainClass.WhoDelet = mainClass.WhoDelet;
                        existingMainClass.DateOfDelet = mainClass.DateOfDelet;
                        existingMainClass.CTnumber = mainClass.CTnumber;
                        existingMainClass.cheackNumber = mainClass.cheackNumber;
                        existingMainClass.cheackDate = mainClass.cheackDate;
                        existingMainClass.dateofTravel = mainClass.dateofTravel;
                        existingMainClass.Amount = mainClass.Amount;
                        existingMainClass.PreparationAuthority = mainClass.PreparationAuthority;
                        existingMainClass.DateOfAddingbyCutterEmployer = mainClass.DateOfAddingbyCutterEmployer;
                        existingMainClass.DepotName = mainClass.DepotName;
                        existingMainClass.Import = mainClass.Import;
                        existingMainClass.Stamps = mainClass.Stamps;
                        existingMainClass.TransportationLoads = mainClass.TransportationLoads;
                        //// Update BankName based on BankListId
                        existingMainClass.BankListId = mainClass.BankListId;
                        existingMainClass.BankName = existingMainClass.BankList?.Name;
                        existingMainClass.A = mainClass.A;
                        existingMainClass.B = mainClass.B;
                        existingMainClass.C= mainClass.C;
                        existingMainClass.checkDate = mainClass.checkDate;


                        // Save changes to the database
                        await _context.SaveChangesAsync();
                        TempData["sucess"] = "تم التعديل بنجاح";
                        // Create a new record in the log table with the old values
                       

                        
                        await _context.SaveChangesAsync();
                        TempData["success"] = "تم التعديل بنجاح";
                        return RedirectToAction(nameof(Index));

                    }
                }    
            }
            ViewData["BankListId"] = new SelectList(_context.BankList, "BankListId", "Name", mainClass.BankListId);
            // Model is not valid or the existingMainClass is not found, return the view with the current mainClass object
            return View(mainClass);
        }

        [Authorize(Roles = "Super Admin ,Admin")]
        // GET: MainClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MainClass == null)
            {
                return NotFound();
            }

            var mainClass = await _context.MainClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mainClass == null)
            {
                return NotFound();
            }

            return View(mainClass);
        }
        [Authorize(Roles = "Super Admin ,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
   
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mainClass = await _context.MainClass.FindAsync(id);

            string userName1 = HttpContext.User.Identity.Name;
            mainClass.WhoDelet = userName1;
            mainClass.DateOfDelet = DateTime.Now;
            if (mainClass != null)
            {
                mainClass.IsDeleted = true;
                //mainClass.CTnumber = mainClass.CTnumber + "A";
            }
            await _context.SaveChangesAsync();
            TempData["success"] = "تم الحذف بنجاح";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBoolean(int id)
        {
            try
            {
                // Fetch the item from your data source using the id
                var item = await _context.MainClass.FindAsync(id);

                if (item != null)
                {
                    // Toggle the Ischecked property
                    item.Ischecked = !item.Ischecked;

                    // Update your data source
                    await _context.SaveChangesAsync();

                    // Fetch the updated item again from the database
                    var updatedItem = await _context.MainClass.FindAsync(id);
                    bool isUpdated = updatedItem.Ischecked; // Check if the value is updated

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Item not found." });
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return Json(new { success = false, message = ex.Message });
            }
        }


        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> DeletedRecords(int Id, DateTime? startDate, DateTime? endDate, string SearchCTNum, string SearchBankName, string SearchPreparationAuthority, string SearchDepotName, string SearchFormCuttingNumber, string SearchProductType, string SearchCutAmount, int pg = 1)
        {
            // Use LINQ to get a list of all active items in the 'MainClass' entity set.
            var searchData = _context.MainClass
             .Include(mc => mc.BankList)
            .Where(mc => mc.IsDeleted);

            // Apply other search criteria as needed
            if (!string.IsNullOrEmpty(SearchCTNum))
            {
                searchData = searchData.Where(g => g.CTnumber.Contains(SearchCTNum));
            }

            if (!string.IsNullOrEmpty(SearchBankName))
            {
                searchData = searchData.Where(g => g.BankList.Name.Contains(SearchBankName));
            }

            if (!string.IsNullOrEmpty(SearchPreparationAuthority))
            {
                searchData = searchData.Where(g => g.PreparationAuthority.Contains(SearchPreparationAuthority));
            }

            if (!string.IsNullOrEmpty(SearchDepotName))
            {
                searchData = searchData.Where(g => g.DepotName.Contains(SearchDepotName));
            }

            // If a start date for the search has been provided, filter the search data to only include items with a date on or after the start date.
            if (startDate != null)
            {
                searchData = searchData.Where(x => x.CTdate >= startDate.Value.Date);
            }

            // If an end date for the search has been provided, filter the search data to only include items with a date before the day after the end date (to include items on the end date).
            if (endDate != null)
            {
                var nextDay = endDate.Value.Date.AddDays(1);
                searchData = searchData.Where(e => e.CTdate < nextDay);
            }

            // Get a list of all distinct product types in the 'KarkhDepot' entity set.
            var depotName = _context.MainClass
           .Select(e => e.DepotName)
           .AsEnumerable()
           .Distinct(StringComparer.OrdinalIgnoreCase)
           .OrderBy(e => e)
           .ToList();

            // Set the page size to 100.
            const int pageSize = 100;

            // If the page number provided is less than 1, set it to 1.
            if (pg < 1)
                pg = 1;

            // Get the total count of items in the filtered search data.
            int rescCount = searchData.Count();

            // Create a new Pager object with the total count of items, current page number, and page size.
            var pager = new Pager(rescCount, pg, pageSize);

            // Calculate the number of items to skip based on the page number and page size.
            int recSkip = (pg - 1) * pageSize;

            // Get the data to display on the current page based on the number of items to skip and the page size.
            var data = searchData.Skip(recSkip).Take(pager.PageSize).ToList();

            // Set the ViewBag variable for the Pager object to use in the view.
            this.ViewBag.Pager = pager;


            ViewBag.SearchCTNum = SearchCTNum; // Pass the search query value to the view
            ViewBag.SearchBankName = SearchBankName; // Pass the location query value to the view
            ViewBag.SearchPreparationAuthority = SearchPreparationAuthority; // Pass the vinNo query value to the view
            ViewBag.SearchDepotName = SearchDepotName; // Pass the fromDate query value to the view


            return View("DeletedRecords", data);
        }

        //public async Task<IActionResult> DeletedRecords(int id)
        //{
        //    var deletedRecords = await _context.MainClass
        //        .Where(mc => mc.IsDeleted)
        //        .ToListAsync();

        //    return View("DeletedRecords", deletedRecords);
        //}
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> PermanentlyDelete(int id)
        {
            var mainClass = await _context.MainClass.FindAsync(id);

            if (mainClass != null)
            {
                _context.MainClass.Remove(mainClass);
                await _context.SaveChangesAsync();
                TempData["success"] = "تم الحذف النهائي بنجاح";
            }

            return RedirectToAction(nameof(DeletedRecords));
        }
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> Restore(int id)
        {
            var mainClass = await _context.MainClass.FindAsync(id);

            if (mainClass != null)
            {
                mainClass.IsDeleted = false;
                
                await _context.SaveChangesAsync();
                TempData["success"] = "تم استعادة البيانات بنجاح";
            }

            return RedirectToAction(nameof(DeletedRecords));
        }



        private bool MainClassExists(int id)
        {
          return (_context.MainClass?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
