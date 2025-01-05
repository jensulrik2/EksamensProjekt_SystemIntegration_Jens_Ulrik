namespace TilmeldAktivitet
{
    // Aktivitetsklasse
    public class Aktivitet
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public decimal MedlemsPris { get; set; }
        public decimal IkkeMedlemsPris { get; set; }   
    }
}