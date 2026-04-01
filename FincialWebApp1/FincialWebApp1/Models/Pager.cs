using System.ComponentModel;


namespace FinancialWebApp1.Models
{
    // The Pager class represents a pagination system for a list of items
    public class Pager
    {
        // The total number of items in the list
        public int TotalItems { get; private set; }

        // The current page being viewed
        public int CurrentPage { get; private set; }

        // The number of items to display on each page
        public int PageSize { get; private set; }

        // The total number of pages based on the total items and page size
        public int TotalPages { get; private set; }

        // The starting page number in the pagination bar
        public int StartPage { get; private set; }

        // The ending page number in the pagination bar
        public int EndPage { get; private set; }

        // Default constructor with no parameters
        public Pager()
        {

        }

        // Constructor with parameters
        public Pager(int totalItems, int page, int pageSize = 10)
        {
            // Calculate the total number of pages based on the total items and page size
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            // Set the current page based on the provided page parameter
            int currentPage = page;

            // Calculate the starting and ending pages for the pagination bar
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            // If the starting page is less than or equal to 0, adjust the ending page and starting page accordingly
            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            // If the ending page is greater than the total number of pages, adjust the ending and starting pages accordingly
            if (endPage > totalPages)
            {
                endPage = totalPages;

                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            // Set the properties of the Pager object based on the calculated values
            TotalItems = totalPages;
            CurrentPage = currentPage;
            PageSize = pageSize;
            StartPage = startPage;
            EndPage = endPage;
            TotalPages = totalPages;
        }
    }
}