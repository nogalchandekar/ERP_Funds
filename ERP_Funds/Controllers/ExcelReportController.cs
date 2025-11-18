using ClosedXML.Excel;
using ERP_Funds.DAL;
using ERP_Funds.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ERP_Funds.Controllers
{
	public class ExcelReportController : Controller
	{
		ExcelReport_DAL reportDAL = new ExcelReport_DAL();

		// GET: ExcelReport
		public ActionResult Index()
		{
			// Get active customer list for dropdown
			var customerList = reportDAL.GetCustomerList();

			if (customerList != null && customerList.Count > 0)
				ViewBag.CustomerList = new SelectList(customerList, "C_Id", "CustomerName");
			else
				ViewBag.CustomerList = new SelectList(new List<SelectListItem>
				{
					new SelectListItem { Value = "", Text = "-- No Active Customers Found --" }
				}, "Value", "Text");

			return View();
		}

		public JsonResult GetFilteredList(int? CustomerId)
		{
			var list = reportDAL.getExcelList(CustomerId);
			return Json(list, JsonRequestBehavior.AllowGet);
		}
		public ActionResult DownloadLoanExcel(int? CustomerId = null, string LoanNo = null)
		{
			try
			{
				// -------------------------------
				// 1️⃣ GET RAW DATA
				// -------------------------------
				var rawData = reportDAL.getExcelReport(CustomerId);

				// -------------------------------
				// 2️⃣ FILTER BY LoanNo
				// -------------------------------
				if (!string.IsNullOrEmpty(LoanNo))
				{
					rawData = rawData.Where(x => x.LoanNo == LoanNo).ToList();
				}

				if (rawData == null || rawData.Count == 0)
				{
					return Content("No data found for given filters.");
				}

				// -------------------------------
				// 3️⃣ SUMMARY (only FIRST ROW)
				// -------------------------------
				var summary = rawData.FirstOrDefault();

				// -------------------------------
				// 4️⃣ DAILY COLLECTION LIST (ALL ROWS)
				// -------------------------------
				var collection = rawData.OrderBy(x => x.TodaysDate).ToList();


				using (var workbook = new XLWorkbook())
				{
					var ws = workbook.Worksheets.Add("Loan Report");
					int row = 1;

					// ---------------------------------------------------------
					// TITLE
					// ---------------------------------------------------------
					ws.Cell(row, 1).Value = "Loan Report";
					ws.Range(row, 1, row, 11).Merge().Style
						.Font.SetBold()
						.Font.SetFontSize(18)
						.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
						.Fill.SetBackgroundColor(XLColor.Yellow);

					row += 2;


					// ---------------------------------------------------------
					// MAIN SUMMARY HEADER
					// ---------------------------------------------------------
					string[] headers1 =
					{
				"Sr No", "Customer Name", "Loan No",
				"Loan Amount", "Duration", "Percentage",
				"Deducted Amount", "Given Amount",
				"Daily Installment", "Total Paid", "Total Pending"
			};

					for (int i = 0; i < headers1.Length; i++)
					{
						ws.Cell(row, i + 1).Value = headers1[i];
						ws.Cell(row, i + 1).Style.Font.SetBold()
								.Font.SetFontColor(XLColor.White)
								.Fill.SetBackgroundColor(XLColor.FromHtml("#4A90E2"))
								.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
								.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
					}

					row++;

					// ---------------------------------------------------------
					// MAIN SUMMARY DATA (SINGLE ROW)
					// ---------------------------------------------------------
					ws.Cell(row, 1).Value = 1;
					ws.Cell(row, 2).Value = summary.CustomerName;
					ws.Cell(row, 3).Value = summary.LoanNo;
					ws.Cell(row, 4).Value = summary.LoanAmount;
					ws.Cell(row, 5).Value = summary.LoanDurationDays;
					ws.Cell(row, 6).Value = summary.LoanInterest;
					ws.Cell(row, 7).Value = summary.DeductAmount;
					ws.Cell(row, 8).Value = summary.AmountGivenToCustomer;
					ws.Cell(row, 9).Value = summary.DailyReturn;
					ws.Cell(row, 10).Value = collection.Sum(x => x.AmountPaidToday); // Total Paid
					ws.Cell(row, 11).Value = summary.PendingAmount - collection.Sum(x => x.AmountPaidToday);

					row += 3;


					// ---------------------------------------------------------
					// DAILY COLLECTION HEADER
					// ---------------------------------------------------------
					string[] detailHeaders =
					{
				"Sr No", "Date", "AmountPaidToday",
				"Daily Installment", "Total Paid",
				"Interest", "Pending Amount",
				"Days Paid", "Remaining Days",
				"Pending Amount", "Date"
			};

					for (int i = 0; i < detailHeaders.Length; i++)
					{
						ws.Cell(row, i + 1).Value = detailHeaders[i];
						ws.Cell(row, i + 1).Style.Font.SetBold()
								.Font.SetFontColor(XLColor.White)
								.Fill.SetBackgroundColor(XLColor.FromHtml("#003366"))
								.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
								.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
					}

					row++;

					// ---------------------------------------------------------
					// DAILY COLLECTION DATA
					// ---------------------------------------------------------
					int sr = 1;
					foreach (var c in collection)
					{
						ws.Cell(row, 1).Value = sr++;
						ws.Cell(row, 2).Value = c.TodaysDate?.ToString("dd-MM-yyyy");
						ws.Cell(row, 3).Value = c.AmountPaidToday;
						ws.Cell(row, 4).Value = c.PerDayInstallment;
						ws.Cell(row, 5).Value = c.TotalPaid;
						ws.Cell(row, 6).Value = summary.LoanInterest;
						ws.Cell(row, 7).Value = c.PendingAmount;
						ws.Cell(row, 8).Value = c.DaysPaid;
						ws.Cell(row, 9).Value = c.RemainingDays;
						ws.Cell(row, 10).Value = c.PendingAmount;
						ws.Cell(row, 11).Value = c.TodaysDate?.ToString("dd-MM-yyyy");

						for (int col = 1; col <= 11; col++)
						{
							ws.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
									.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
						}

						row++;
					}

					// AUTO FIT
					ws.Columns().AdjustToContents();

					// RETURN FILE
					using (var stream = new MemoryStream())
					{
						workbook.SaveAs(stream);

						return File(stream.ToArray(),
							"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
							$"LoanReport_{summary.LoanNo}.xlsx");
					}
				}
			}
			catch (Exception ex)
			{
				return new HttpStatusCodeResult(500, "Excel Error: " + ex.Message);
			}
		}

	}
}

