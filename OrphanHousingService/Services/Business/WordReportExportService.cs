using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OrphanHousingService.Models.Reports;

namespace OrphanHousingService.Services.Business
{
    public class WordReportExportService
    {
        public void ExportToWord(ReportModel report, string filePath)
        {
            using var document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            var body = mainPart.Document.Body!;

            body.Append(CreateParagraph(report.ReportTitle, bold: true, size: 32));
            body.Append(CreateParagraph(
                $"Дата генерации: {report.GeneratedAt:dd.MM.yyyy HH:mm}",
                size: 22));
            body.Append(CreateParagraph(
                $"Период: {report.Period.DisplayText}",
                size: 22));
            body.Append(CreateParagraph(string.Empty));

            body.Append(CreateParagraph("Квартиры", bold: true, size: 26));
            body.Append(CreateStatisticsTable(new (string, string)[]
            {
                ("Всего квартир", report.ApartmentStatistics.TotalApartments.ToString()),
                ("Передано в спец. жилой фонд", report.ApartmentStatistics.SpecialHousingFund.ToString()),
                ("Передано в социальный найм", report.ApartmentStatistics.SocialHousing.ToString()),
                ("Передано в жилищный фонд (распределено)", report.ApartmentStatistics.HousingFund.ToString())
            }));
            body.Append(CreateParagraph(string.Empty));

            body.Append(CreateParagraph("Договоры", bold: true, size: 26));
            body.Append(CreateStatisticsTable(new (string, string)[]
            {
                ("Всего договоров", report.ContractStatistics.TotalContracts.ToString()),
                ("Активных договоров", report.ContractStatistics.ActiveContracts.ToString()),
                ("Перезаключённых договоров", report.ContractStatistics.RenegotiatedContracts.ToString()),
                ("Продлённых договоров", report.ContractStatistics.ExtendedContracts.ToString()),
                ("Сокращённых договоров", report.ContractStatistics.TerminatedContracts.ToString())
            }));

            if (!string.IsNullOrWhiteSpace(report.AdditionalInfo.Notes))
            {
                body.Append(CreateParagraph(string.Empty));
                body.Append(CreateParagraph("Дополнительная информация", bold: true, size: 26));
                body.Append(CreateParagraph(report.AdditionalInfo.Notes, size: 22));
            }

            mainPart.Document.Save();
        }

        private static Paragraph CreateParagraph(string text, bool bold = false, int size = 24)
        {
            var runProperties = new RunProperties();
            if (bold)
                runProperties.Append(new Bold());
            runProperties.Append(new FontSize { Val = size.ToString() });

            return new Paragraph(new Run(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
        }

        private static Table CreateStatisticsTable((string Label, string Value)[] rows)
        {
            var table = new Table(
                new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder { Val = BorderValues.Single, Size = 4 },
                        new RightBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })));

            foreach (var (label, value) in rows)
            {
                table.Append(new TableRow(
                    CreateTableCell(label, bold: true),
                    CreateTableCell(value)));
            }

            return table;
        }

        private static TableCell CreateTableCell(string text, bool bold = false)
        {
            var runProperties = new RunProperties();
            if (bold)
                runProperties.Append(new Bold());

            var paragraph = new Paragraph(new Run(runProperties, new Text(text)));
            return new TableCell(paragraph);
        }
    }
}
