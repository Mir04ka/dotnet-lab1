using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TravelAgency.Domain.DTOs;
using TravelAgency.Domain.Entities;

namespace TravelAgency.Reports;

public class WordReportService
{
    // 1. Отчет по всем данным таблицы "Туры"
    public void GenerateToursReport(string filePath, IEnumerable<Tour> tours)
    {
        using var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        AddHeading(body, "ОТЧЕТ: Полный список туров");

        var table = new Table();
        AddTableHeader(table, "№ Тура", "Название", "Направление", "Дней", "Начало", "Отель (*)", "Мест");

        foreach (var t in tours)
        {
            var row = new TableRow();
            row.Append(
                CreateCell(t.TourNumber),
                CreateCell(t.TourName),
                CreateCell(t.Destination),
                CreateCell(t.DurationDays.ToString()),
                CreateCell(t.StartDate.ToShortDateString()),
                CreateCell(t.HotelClass.ToString()),
                CreateCell(t.AvailableSeats.ToString())
            );
            table.Append(row);
        }

        body.Append(table);
        mainPart.Document.Save();
    }

    // 2. Отчет по запросу: Туры конкретного заказчика
    public void GenerateCustomerToursReport(string filePath, string customerName, IEnumerable<CustomerTourDto> items)
    {
        using var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        AddHeading(body, $"ОТЧЕТ: Туры клиента «{customerName}»");

        var table = new Table();
        AddTableHeader(table, "№ Договора", "Дата", "Тур", "Дней", "Стоимость (руб.)");

        decimal total = 0;
        foreach (var item in items)
        {
            total += item.TotalCost;
            var row = new TableRow();
            row.Append(
                CreateCell(item.ContractNumber),
                CreateCell(item.ContractDate.ToShortDateString()),
                CreateCell(item.TourName),
                CreateCell(item.DurationDays.ToString()),
                CreateCell(item.TotalCost.ToString("N2"))
            );
            table.Append(row);
        }

        body.Append(table);
        AddParagraph(body, $"\nИТОГО по клиенту: {total:N2} руб.", true);
        mainPart.Document.Save();
    }

    // 3. Отчет по группирующему запросу с промежуточными и общим итогами
    public void GenerateEmployeeFinancialSummaryReport(string filePath, IEnumerable<EmployeeDailySalesDto> sales)
    {
        using var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        AddHeading(body, "ГРУППИРУЮЩИЙ ОТЧЕТ: Выручка по сотрудникам и датам");

        var groups = sales.GroupBy(s => s.EmployeeName);
        decimal grandTotal = 0;

        foreach (var empGroup in groups)
        {
            AddParagraph(body, $"Сотрудник: {empGroup.Key}", true);
            var table = new Table();
            AddTableHeader(table, "Дата договоров", "Сумма за дату (руб.)");

            decimal employeeTotal = 0;
            foreach (var item in empGroup)
            {
                employeeTotal += item.DailyTotal;
                var row = new TableRow();
                row.Append(
                    CreateCell(item.ContractDate.ToShortDateString()),
                    CreateCell(item.DailyTotal.ToString("N2"))
                );
                table.Append(row);
            }
            body.Append(table);
            AddParagraph(body, $"Итого по сотруднику {empGroup.Key}: {employeeTotal:N2} руб.\n", true);
            grandTotal += employeeTotal;
        }

        AddParagraph(body, $"============================================", false);
        AddParagraph(body, $"ОБЩИЙ ИТОГ ПО ВСЕМ СОТРУДНИКАМ: {grandTotal:N2} руб.", true);

        mainPart.Document.Save();
    }

    private static void AddHeading(Body body, string text)
    {
        var p = body.AppendChild(new Paragraph());
        var run = p.AppendChild(new Run());
        run.AppendChild(new Text(text));
        run.RunProperties = new RunProperties(new Bold(), new FontSize { Val = "32" });
    }

    private static void AddParagraph(Body body, string text, bool bold)
    {
        var p = body.AppendChild(new Paragraph());
        var run = p.AppendChild(new Run(new Text(text)));
        if (bold) run.RunProperties = new RunProperties(new Bold());
    }

    private static void AddTableHeader(Table table, params string[] headers)
    {
        var row = new TableRow();
        foreach (var header in headers)
        {
            var cell = CreateCell(header);
            var run = cell.Descendants<Run>().FirstOrDefault();
            if (run != null) run.RunProperties = new RunProperties(new Bold());
            row.Append(cell);
        }
        table.Append(row);
    }

    private static TableCell CreateCell(string text)
    {
        return new TableCell(new Paragraph(new Run(new Text(text))));
    }
}