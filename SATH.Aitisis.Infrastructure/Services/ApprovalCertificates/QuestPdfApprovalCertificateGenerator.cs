using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SATH.Aitisis.App.Services.ApprovalCertificates;
using QuestPDF.Elements;


namespace SATH.Aitisis.Infrastructure.Services.ApprovalCertificates
{
    public sealed class QuestPdfApprovalCertificateGenerator : IApprovalCertificatePdfGenerator
    {
        public byte[] Generate(ApprovalCertificateData data)
        {
            // sos 
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Element(header =>
                        {
                            header.Column(column =>
                            {
                                column.Item().Text("SATH-Aitisis").FontSize(22).Bold();

                                column.Item().Text("Approval Certificate").FontSize(16).SemiBold();

                                column.Item().PaddingTop(5).Text("This certificate was generated electronically by the SATH-Aitisis system.").FontSize(9).FontColor(Colors.Grey.Darken1);
                            });
                        });

                    page.Content()
                        .PaddingVertical(25)
                        .Column(column =>
                        {
                            column.Spacing(18);

                            column.Item()
                                .Element(container =>
                                {
                                    container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(15)
                                        .Column(section =>
                                        {
                                            section.Spacing(8);

                                            section.Item().Text("Certificate Information").FontSize(14).Bold();

                                            AddKeyValue(section, "Protocol Number", data.ProtocolNumber);

                                            AddKeyValue(section, "Application ID", $"#{data.ApplicationId}");

                                            AddKeyValue(section, "Application Type", data.ApplicationTypeName);

                                            AddKeyValue(section, "Status", "Approved");

                                            AddKeyValue(section, "Submitted On", FormatDate(data.SubmissionDate));

                                            AddKeyValue(section, "Approved On", FormatDate(data.ApprovalDate));
                                        });
                                });

                            column.Item()
                                .Element(container =>
                                {
                                    container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(15)
                                        .Column(section =>
                                        {
                                            section.Spacing(8);

                                            section.Item().Text("Citizen").FontSize(14).Bold();

                                            AddKeyValue(section, "Full Name", data.CitizenFullName);

                                            AddKeyValue(section, "Email", data.CitizenEmail);
                                        });
                                });

                            column.Item()
                                .Element(container =>
                                {
                                    container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(15)
                                        .Column(section =>
                                        {
                                            section.Spacing(8);

                                            section.Item().Text("Approved By").FontSize(14).Bold();

                                            AddKeyValue(section, "Employee", data.EmployeeFullName);
                                        });
                                });

                            column.Item()
                                .Element(container =>
                                {
                                    container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(15)
                                        .Column(section =>
                                        {
                                            section.Spacing(8);

                                            section.Item().Text("Submitted Information").FontSize(14).Bold();

                                            if (!data.Fields.Any())
                                            {
                                                section.Item()
                                                    .Text("No additional application fields were submitted.")
                                                    .FontSize(10)
                                                    .FontColor(Colors.Grey.Darken1);
                                            }
                                            else
                                            {
                                                foreach (var field in data.Fields.OrderBy(x => x.DisplayOrder))
                                                {
                                                    AddKeyValue(
                                                        section,
                                                        field.Label,
                                                        string.IsNullOrWhiteSpace(field.Value) ? "-" : field.Value);
                                                }
                                            }
                                        });
                                });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ").FontSize(9).FontColor(Colors.Grey.Darken1);

                            text.Span(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")).FontSize(9).FontColor(Colors.Grey.Darken1);

                            text.Span(" UTC").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                });
            }).GeneratePdf();
        }

        private static void AddKeyValue(ColumnDescriptor column, string label, string value)
        {
            column.Item()
                .Row(row =>
                {
                    row.RelativeItem(1)
                        .Text(label)
                        .FontSize(10)
                        .SemiBold();

                    row.RelativeItem(2).Text(value).FontSize(10);
                });
        }

        private static string FormatDate(DateTime date)
        {
            return date.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
        }
    }
}