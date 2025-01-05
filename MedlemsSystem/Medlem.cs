namespace Eksamens_Projekt_SystemIntegration_Jens_Ulrik
{
    public class Medlem
    {
        public int MedlemsNummer { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public string Adresselinie1 { get; set; }
        public string Adresselinie2 { get; set; }
        public string PostNr { get; set; }
        public string ByNavn { get; set; }
        public string MailAdresse { get; set; }
        public bool PrivatFirma { get; set; }
        public bool AktivPassiv { get; set; }
        public bool KontingentBetalt { get; set; }
        public bool NyhedsBrev { get; set; }
        public string DatoBrev { get; set; } // Format: dd.måned.åååå
    }
}