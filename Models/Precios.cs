namespace CentroDeCopias.Models
{
    public static class Precios
    {
        // Descuento para usuarios con rol Escuela (15%)
        public const decimal DescuentoEscuela = 0.15m;

        public static decimal AplicarDescuento(decimal monto, bool esEscuela)
            => esEscuela ? Math.Round(monto * (1 - DescuentoEscuela), 2) : monto;

        public static decimal PrecioAnillado(string? anillado) => anillado switch
        {
            "Chico" => 1500m,
            "Mediano" => 2500m,
            "Grande" => 4000m,
            _ => 0m
        };

        public static decimal CalcularImpresion(
            int paginas, int copias, bool color, bool dobleFaz,
            string? anillado, bool esEscuela)
        {
            int hojasPorCopia = dobleFaz ? (int)Math.Ceiling(paginas / 2.0) : paginas;
            int hojasTotales = hojasPorCopia * copias;

            decimal precioPorHoja;
            if (hojasTotales <= 20)
                precioPorHoja = color ? 100 : 80;
            else if (hojasTotales <= 100)
                precioPorHoja = color ? 70 : 60;
            else
                precioPorHoja = color ? 50 : 40;

            decimal impresion = precioPorHoja * hojasTotales;
            decimal anillados = PrecioAnillado(anillado) * copias;

            return AplicarDescuento(impresion + anillados, esEscuela);
        }
    }
}