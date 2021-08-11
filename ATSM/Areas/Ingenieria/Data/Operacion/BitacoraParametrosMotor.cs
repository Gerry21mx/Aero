using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace ATSM.Ingenieria {
	public class BitacoraParametrosMotor {
		private static SqlConnection Conexion = DataBase.Conexion();
		public int IdBitacora { get; set; }
		public int? IdMotor1 { get; set; }
		public int ALT1 { get; set; }
		public decimal EPR1 { get; set; }
		public decimal N11 { get; set; }
		public int EGT1 { get; set; }
		public decimal N21 { get; set; }
		public int FF1 { get; set; }
		public int OT1 { get; set; }
		public int OP1 { get; set; }
		public bool Bleed1 { get; set; }
		public int OAT1 { get; set; }
		public int TRQ1 { get; set; }
		public int RPM1 { get; set; }
		public decimal VIB1 { get; set; }
		public decimal? ENG1 { get; set; }
		public decimal? PGU1 { get; set; }
		public decimal? APU1 { get; set; }
		public int? IdMotor2 { get; set; }
		public int ALT2 { get; set; }
		public decimal EPR2 { get; set; }
		public decimal N12 { get; set; }
		public int EGT2 { get; set; }
		public decimal N22 { get; set; }
		public int FF2 { get; set; }
		public int OT2 { get; set; }
		public int OP2 { get; set; }
		public bool Bleed2 { get; set; }
		public int OAT2 { get; set; }
		public int TRQ2 { get; set; }
		public int RPM2 { get; set; }
		public decimal VIB2 { get; set; }
		public decimal? ENG2 { get; set; }
		public decimal? PGU2 { get; set; }
		public decimal? APU2 { get; set; }
		public bool Valid { get; set; }
        public BitacoraParametrosMotor() { Inicializar(); }
        public BitacoraParametrosMotor(int idBitacora) {
            Inicializar();
            if (idBitacora > 0) {
                SqlCommand comando = new SqlCommand($"SELECT * FROM BitacoraParametrosMotor WHERE IdBitacora = @idBitacora", Conexion);
                comando.Parameters.Add(new SqlParameter("@idBitacora", idBitacora));
                SetDatos(comando);
            }
        }
        [JsonConstructor]
        public BitacoraParametrosMotor(int idBitacora, int? idMotor1, int aLT1, decimal ePR1 = 0, decimal n11 = 0, int eGT1 = 0, decimal n21 = 0, int fF1 = 0, int oT1 = 0, int oP1 = 0, bool bleed1 = false, int oAT1 = 0, int tRQ1 = 0, int rPM1 = 0, decimal vIB1 = 0, decimal? eNG1 = null, decimal? pGU1 = null, decimal? aPU1 = null, int? idMotor2 = null, int aLT2 = 0, decimal ePR2 = 0, decimal n12 = 0, int eGT2 = 0, decimal n22 = 0, int fF2 = 0, int oT2 = 0, int oP2 = 0, bool bleed2 = false, int oAT2 = 0, int tRQ2 = 0, int rPM2 = 0, decimal vIB2 = 0, decimal? eNG2 = null, decimal? pGU2 = null, decimal? aPU2 = null, bool valid = false) {
            IdBitacora = idBitacora;
            IdMotor1 = idMotor1;
            ALT1 = aLT1;
            EPR1 = ePR1;
            N11 = n11;
            EGT1 = eGT1;
            N21 = n21;
            FF1 = fF1;
            OT1 = oT1;
            OP1 = oP1;
            Bleed1 = bleed1;
            OAT1 = oAT1;
            TRQ1 = tRQ1;
            RPM1 = rPM1;
            VIB1 = vIB1;
            ENG1 = eNG1;
            PGU1 = pGU1;
            APU1 = aPU1;
            IdMotor2 = idMotor2;
            ALT2 = aLT2;
            EPR2 = ePR2;
            N12 = n12;
            EGT2 = eGT2;
            N22 = n22;
            FF2 = fF2;
            OT2 = oT2;
            OP2 = oP2;
            Bleed2 = bleed2;
            OAT2 = oAT2;
            TRQ2 = tRQ2;
            RPM2 = rPM2;
            VIB2 = vIB2;
            ENG2 = eNG2;
            PGU2 = pGU2;
            APU2 = aPU2;
            Valid = valid;
        }
        public Respuesta Save() {
            Respuesta res = new Respuesta($"No se Guardaron los Datos.Faltan Informacion. (CS.{ this.GetType().Name}-Save.Err.00)");
            if (IdBitacora > 0) {
                res.Error = "";
                SqlCommand Cmnd = new SqlCommand($"SELECT IdBitacora FROM BitacoraParametrosMotor WHERE IdBitacora = @idbitacora", Conexion);
                Cmnd.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
                var existe = DataBase.Query(Cmnd);
                res.Mensaje = "BitacoraParametrosMotor ";
                string SqlStr = "";
                bool Insr = false;
                if (existe.Valid) {
                    SqlStr = @"UPDATE BitacoraParametrosMotor SET IdMotor1 = @idmotor1, ALT1 = @alt1, EPR1 = @epr1, N11 = @n11, EGT1 = @egt1, N21 = @n21, FF1 = @ff1, OT1 = @ot1, OP1 = @op1, Bleed1 = @bleed1, OAT1 = @oat1, TRQ1 = @trq1, RPM1 = @rpm1, VIB1 = @vib1, ENG1 = @eng1, PGU1 = @pgu1, APU1 = @apu1, IdMotor2 = @idmotor2, ALT2 = @alt2, EPR2 = @epr2, N12 = @n12, EGT2 = @egt2, N22 = @n22, FF2 = @ff2, OT2 = @ot2, OP2 = @op2, Bleed2 = @bleed2, OAT2 = @oat2, TRQ2 = @trq2, RPM2 = @rpm2, VIB2 = @vib2, ENG2 = @eng2, PGU2 = @pgu2, APU2 = @apu2 WHERE IdBitacora = @idbitacora";
                    res.Mensaje += "Actualizada Correctamente";
                }
                else {
                    if (!string.IsNullOrEmpty(existe.Error)) {
                        res.Error = $"Error al Consultar las existencias coincidentes. (CS.{this.GetType().Name}-Save.Err.01).<br>{ existe.Error}";
                        return res;
                    }
                    SqlStr = @"INSERT INTO BitacoraParametrosMotor(IdBitacora, IdMotor1, ALT1, EPR1, N11, EGT1, N21, FF1, OT1, OP1, Bleed1, OAT1, TRQ1, RPM1, VIB1, ENG1, PGU1, APU1, IdMotor2, ALT2, EPR2, N12, EGT2, N22, FF2, OT2, OP2, Bleed2, OAT2, TRQ2, RPM2, VIB2, ENG2, PGU2, APU2) VALUES(@idbitacora, @idmotor1, @alt1, @epr1, @n11, @egt1, @n21, @ff1, @ot1, @op1, @bleed1, @oat1, @trq1, @rpm1, @vib1, @eng1, @pgu1, @apu1, @idmotor2, @alt2, @epr2, @n12, @egt2, @n22, @ff2, @ot2, @op2, @bleed2, @oat2, @trq2, @rpm2, @vib2, @eng2, @pgu2, @apu2)";
                    res.Mensaje += "Registrada Correctamente";
                    Insr = true;
                }
                SqlCommand Command = new SqlCommand(SqlStr, Conexion);
                Command.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
                Command.Parameters.Add(new SqlParameter("@idmotor1", IdMotor1 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@alt1", ALT1));
                Command.Parameters.Add(new SqlParameter("@epr1", EPR1));
                Command.Parameters.Add(new SqlParameter("@n11", N11));
                Command.Parameters.Add(new SqlParameter("@egt1", EGT1));
                Command.Parameters.Add(new SqlParameter("@n21", N21));
                Command.Parameters.Add(new SqlParameter("@ff1", FF1));
                Command.Parameters.Add(new SqlParameter("@ot1", OT1));
                Command.Parameters.Add(new SqlParameter("@op1", OP1));
                Command.Parameters.Add(new SqlParameter("@bleed1", Bleed1));
                Command.Parameters.Add(new SqlParameter("@oat1", OAT1));
                Command.Parameters.Add(new SqlParameter("@trq1", TRQ1));
                Command.Parameters.Add(new SqlParameter("@rpm1", RPM1));
                Command.Parameters.Add(new SqlParameter("@vib1", VIB1));
                Command.Parameters.Add(new SqlParameter("@eng1", ENG1 ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@pgu1", PGU1 ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@apu1", APU1 ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@idmotor2", IdMotor2 ?? SqlInt32.Null));
                Command.Parameters.Add(new SqlParameter("@alt2", ALT2));
                Command.Parameters.Add(new SqlParameter("@epr2", EPR2));
                Command.Parameters.Add(new SqlParameter("@n12", N12));
                Command.Parameters.Add(new SqlParameter("@egt2", EGT2));
                Command.Parameters.Add(new SqlParameter("@n22", N22));
                Command.Parameters.Add(new SqlParameter("@ff2", FF2));
                Command.Parameters.Add(new SqlParameter("@ot2", OT2));
                Command.Parameters.Add(new SqlParameter("@op2", OP2));
                Command.Parameters.Add(new SqlParameter("@bleed2", Bleed2));
                Command.Parameters.Add(new SqlParameter("@oat2", OAT2));
                Command.Parameters.Add(new SqlParameter("@trq2", TRQ2));
                Command.Parameters.Add(new SqlParameter("@rpm2", RPM2));
                Command.Parameters.Add(new SqlParameter("@vib2", VIB2));
                Command.Parameters.Add(new SqlParameter("@eng2", ENG2 ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@pgu2", PGU2 ?? SqlDecimal.Null));
                Command.Parameters.Add(new SqlParameter("@apu2", APU2 ?? SqlDecimal.Null));
                RespuestaQuery rInUp = DataBase.Execute(Command);
                if (rInUp.Valid) {
                    Valid = true;
                    res.Elemento = this;
                    res.Valid = true;
                }
                else {
                    res.Error = $"Error al Registrar: (CS.{this.GetType().Name}-Save.Err.02)<br>{SqlStr}<br> Error: {rInUp.Error}";
                    return res;
                }
            }
            else {
                if (IdBitacora<=0)
                    res.Error += $"<br>Falta la Bitacora referenciada.";
            }
            return res;
        }
        public Respuesta Delete() {
            Respuesta res = new Respuesta("BitacoraParametrosMotor NO se Elimino");
            SqlCommand Command = new SqlCommand("DELETE BitacoraParametrosMotor WHERE IdBitacora = @idbitacora", Conexion);
            Command.Parameters.Add(new SqlParameter("@idbitacora", IdBitacora));
            var resD = DataBase.Execute(Command);
            if (resD.Valid && resD.Afectados > 0) {
                res.Valid = true;
                res.Error = "";
                res.Mensaje = "Eliminado Correctamente";
                Inicializar();
            }
            else {
                if (!string.IsNullOrEmpty(resD.Error)) {
                    res.Error = resD.Error;
                }
                else {
                    res.Mensaje = "No se encontraron coincidencias para elminar.";
                }
            }
            return res;
        }
        private void SetDatos(SqlCommand Command) {
            RespuestaQuery res = DataBase.Query(Command);
            if (res.Valid) {
                var Registro = res.Row;
                IdBitacora = Registro.IdBitacora;
                IdMotor1 = Registro.IdMotor1;
                ALT1 = Registro.ALT1;
                EPR1 = Registro.EPR1;
                N11 = Registro.N11;
                EGT1 = Registro.EGT1;
                N21 = Registro.N21;
                FF1 = Registro.FF1;
                OT1 = Registro.OT1;
                OP1 = Registro.OP1;
                Bleed1 = Registro.Bleed1;
                OAT1 = Registro.OAT1;
                TRQ1 = Registro.TRQ1;
                RPM1 = Registro.RPM1;
                VIB1 = Registro.VIB1;
                ENG1 = Registro.ENG1;
                PGU1 = Registro.PGU1;
                APU1 = Registro.APU1;
                IdMotor2 = Registro.IdMotor2;
                ALT2 = Registro.ALT2;
                EPR2 = Registro.EPR2;
                N12 = Registro.N12;
                EGT2 = Registro.EGT2;
                N22 = Registro.N22;
                FF2 = Registro.FF2;
                OT2 = Registro.OT2;
                OP2 = Registro.OP2;
                Bleed2 = Registro.Bleed2;
                OAT2 = Registro.OAT2;
                TRQ2 = Registro.TRQ2;
                RPM2 = Registro.RPM2;
                VIB2 = Registro.VIB2;
                ENG2 = Registro.ENG2;
                PGU2 = Registro.PGU2;
                APU2 = Registro.APU2;
                Valid = true;
            }
        }
        private void Inicializar() {
            IdBitacora = 0;
            IdMotor1 = null;
            ALT1 = 0;
            EPR1 = 0;
            N11 = 0;
            EGT1 = 0;
            N21 = 0;
            FF1 = 0;
            OT1 = 0;
            OP1 = 0;
            Bleed1 = false;
            OAT1 = 0;
            TRQ1 = 0;
            RPM1 = 0;
            VIB1 = 0;
            ENG1 = null;
            PGU1 = null;
            APU1 = null;
            IdMotor2 = null;
            ALT2 = 0;
            EPR2 = 0;
            N12 = 0;
            EGT2 = 0;
            N22 = 0;
            FF2 = 0;
            OT2 = 0;
            OP2 = 0;
            Bleed2 = false;
            OAT2 = 0;
            TRQ2 = 0;
            RPM2 = 0;
            VIB2 = 0;
            ENG2 = null;
            PGU2 = null;
            APU2 = null;
            Valid = false;
        }
    }
}