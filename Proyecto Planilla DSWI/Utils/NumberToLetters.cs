using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Planilla_DSWI.Utils
{
    [Serializable]
    public sealed class NumberToLetters
    {
        #region Miembros estAticos

        private const int UNI = 0, DIECI = 1, DECENA = 2, CENTENA = 3;
        private static string[,] _matriz = new string[CENTENA + 1, 10]
            {
            {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
            {" diez"," once"," doce"," trece"," catorce"," quince"," dieciseis"," diecisiete"," dieciocho"," diecinueve"},
            {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
            {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
            };

        private const Char sub = (Char)26;
        //Cambiar acA si se quiere otro comportamiento en los métodos de clase
        public const String SeparadorDecimalSalidaDefault = "con";
        public const String MascaraSalidaDecimalDefault = "00/100. ";
        public const int DecimalesDefault = 2;
        public const bool LetraCapitalDefault = false;
        public const bool ConvertirDecimalesDefault = false;
        public const bool ApocoparUnoParteEnteraDefault = false;
        public const bool ApocoparUnoParteDecimalDefault = false;

        #endregion

        #region Propiedades

        private int _decimales = DecimalesDefault;
        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
        private int _posiciones = DecimalesDefault;
        private String _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private bool _esMascaraNumerica = true;
        private bool _letraCapital = LetraCapitalDefault;
        private bool _convertirDecimales = ConvertirDecimalesDefault;
        private bool _apocoparUnoParteEntera = false;
        private bool _apocoparUnoParteDecimal;

        /// <summary>
        /// Indica la cantidad de decimales que se pasarAn a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con "0"</remarks>
        public int Decimales
        {
            get { return _decimales; }
            set
            {
                if (value > 10) throw new ArgumentException(value.ToString() + " excede el número mAximo de decimales admitidos, solo se admiten hasta 10.");
                _decimales = value;
            }
        }

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public String SeparadorDecimalSalida
        {
            get { return _separadorDecimalSalida; }
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                if (value.Trim().IndexOf(" ") > 0)
                    _apocoparUnoParteEntera = true;
                else _apocoparUnoParteEntera = false;
            }
        }

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public String MascaraSalidaDecimal
        {
            get
            {
                if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    return _mascaraSalidaDecimal;
                else return "";
            }
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de "0" o "#" 
                //que haya al principio de la cadena, y también si es una mAscara numérica
                int i = 0;
                while (i < value.Length
                    && (value[i].ToString() == "0".ToString())
                        | value[i].ToString() == "#".ToString())
                    i++;
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else _esMascaraNumerica = false;
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + ""
                        + value.Substring(_posiciones)
                        .Replace(" ", sub.ToString())
                        .Replace(" ", String.Empty)
                        .Replace(sub.ToString(), "") + "";
                else
                    _mascaraSalidaDecimalInterna = value
                        .Replace(" ", sub.ToString())
                        .Replace(" ", String.Empty)
                        .Replace(sub.ToString(), " ");
            }
        }

        /// <summary>
        /// Indica si la primera letra del resultado debe estAr en mayúscula
        /// </summary>
        public bool LetraCapital
        {
            get { return _letraCapital; }
            set { _letraCapital = value; }
        }

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public bool ConvertirDecimales
        {
            get { return _convertirDecimales; }
            set
            {
                _convertirDecimales = value;
                _apocoparUnoParteDecimal = value;
                if (value)
                {// Si la mAscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                        MascaraSalidaDecimal = "";
                }
                else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    //Si no hay mAscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            }
        }

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public bool ApocoparUnoParteEntera
        {
            get { return _apocoparUnoParteEntera; }
            set { _apocoparUnoParteEntera = value; }
        }

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public bool ApocoparUnoParteDecimal
        {
            get { return _apocoparUnoParteDecimal; }
            set { _apocoparUnoParteDecimal = value; }
        }

        #endregion

        #region Constructores

        public NumberToLetters()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public NumberToLetters(bool ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, bool LetraCapital)
        {
            if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
                this.MascaraSalidaDecimal = MascaraSalidaDecimal;
            if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
                _separadorDecimalSalida = SeparadorDecimalSalida;
            _letraCapital = LetraCapital;
            _convertirDecimales = ConvertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public String ToCustomCardinal(Double Numero)
        { return Convertir((decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, _apocoparUnoParteDecimal); }

        public String ToCustomCardinal(String Numero)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
                return ToCustomCardinal(dNumero);
            else throw new ArgumentException(" " + Numero + " no es un número valido.");
        }

        public String ToCustomCardinal(decimal Numero)
        { return ToCardinal((Numero)); }

        public String ToCustomCardinal(int Numero)
        { return Convertir((decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, false); }

        #endregion

        #region Conversores estAticos

        public static String ToCardinal(int Numero)
        {
            return Convertir((decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static String ToCardinal(Double Numero)
        {
            return ToCardinal((decimal)Numero);
        }

        public static String ToCardinal(String Numero, CultureInfo ReferenciaCultural)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
                return ToCardinal(dNumero);
            else throw new ArgumentException(" " + Numero + "  no es un número valido.");
        }

        public static String ToCardinal(String Numero)
        {
            return NumberToLetters.ToCardinal(Numero, CultureInfo.CurrentCulture);
        }

        public static String ToCardinal(decimal Numero)
        {
            return Convertir(Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        #endregion

        private static String Convertir(decimal Numero, int Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, bool EsMascaraNumerica, bool LetraCapital, bool ConvertirDecimales, bool ApocoparUnoParteEntera, bool ApocoparUnoParteDecimal)
        {
            long Num;
            int terna, centenaTerna, decenaTerna, unidadTerna, iTerna;
            String cadTerna;
            StringBuilder Resultado = new StringBuilder();

            Num = (long)Math.Abs(Numero);

            if (Num >= 1000000000000 || Num < 0) throw new ArgumentException("El numero " + Numero.ToString() + " excedio los limites del conversor: [0;1.000.000.000.000)");
            if (Num == 0)
                Resultado.Append(" cero");
            else
            {
                iTerna = 0;
                while (Num > 0)
                {
                    iTerna++;
                    cadTerna = String.Empty;
                    terna = (int)(Num % 1000);

                    centenaTerna = (int)(terna / 100);
                    decenaTerna = terna % 100;
                    unidadTerna = terna % 10;

                    if ((decenaTerna > 0) && (decenaTerna < 10))
                        cadTerna = _matriz[UNI, unidadTerna] + cadTerna;
                    else if ((decenaTerna >= 10) && (decenaTerna < 20))
                        cadTerna = cadTerna + _matriz[DIECI, unidadTerna];
                    else if (decenaTerna == 20)
                        cadTerna = cadTerna + " veinte";
                    else if ((decenaTerna > 20) && (decenaTerna < 30))
                        cadTerna = " veinti" + _matriz[UNI, unidadTerna].Substring(1);
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                        if (unidadTerna != 0)
                            cadTerna = _matriz[DECENA, (int)(decenaTerna / 10)] + " y" + _matriz[UNI, unidadTerna] + cadTerna;
                        else
                            cadTerna += _matriz[DECENA, (int)(decenaTerna / 10)];

                    switch (centenaTerna)
                    {
                        case 1:
                            if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                            else cadTerna = " cien" + cadTerna;
                            break;
                        case 5:
                        case 7:
                        case 9:
                            cadTerna = _matriz[CENTENA, (int)(terna / 100)] + cadTerna;
                            break;
                        default:
                            if ((int)(terna / 100) > 1) cadTerna = _matriz[UNI, (int)(terna / 100)] + "cientos" + cadTerna;
                            break;
                    }
                    //Reemplazo el "uno" por "un" si no es en las unidades o si se solicito apocopar
                    if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                        cadTerna = cadTerna.Replace("veintiuno", "veintiun");
                    else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    //Acentuo "veintidos", "veintitres" y "veintiseis"
                    else if (decenaTerna == 22) cadTerna = cadTerna.Replace("veintidos", "veintidos");
                    else if (decenaTerna == 23) cadTerna = cadTerna.Replace("veintitres", "veintitres");
                    else if (decenaTerna == 26) cadTerna = cadTerna.Replace("veintiseis", "veintiseis");

                    //Completo miles y millones
                    switch (iTerna)
                    {
                        case 3:
                            if (Numero < 2000000) cadTerna += " millon";
                            else cadTerna += " millones";
                            break;
                        case 2:
                        case 4:
                            if (terna > 0) cadTerna += " mil";
                            break;
                    }
                    Resultado.Insert(0, cadTerna);
                    Num = (int)(Num / 1000);
                } //while
            }

            //Se agregan los decimales si corresponde
            if (Decimales > 0)
            {
                Resultado.Append(" " + SeparadorDecimalSalida + " ");
                int EnteroDecimal = (int)Math.Round((Double)(Numero - (long)Numero) * Math.Pow(10, Decimales), 0);
                if (ConvertirDecimales)
                {
                    bool esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                    Resultado.Append(Convertir((decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                        + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
                }
                else if (EnteroDecimal > 0)
                {
                    Resultado.Append(EnteroDecimal.ToString() + "/100");
                }
                else
                {
                    if (EsMascaraNumerica) Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                    else Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
                }

            }
            //Se pone la primer letra en mayuscula si corresponde y se retorna el resultado
            if (LetraCapital)
                return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
            else
                return Resultado.ToString().Substring(1);
        }
    }
} 