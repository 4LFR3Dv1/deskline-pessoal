using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

[assembly: AssemblyTitle("Deskline Pessoal")]
[assembly: AssemblyDescription("Iniciador pessoal para Deskline no Windows")]
[assembly: AssemblyVersion("1.0.0.0")]

internal static class Motor
{
    internal const string Hash = "c0cb517ace3f66943b252c085e815cfc64e5f4c8d5478e4b67247a1305467376";

    internal static string Codigo(string entrada)
    {
        string codigo = Regex.Replace(entrada, @"[\s-]", "").ToLowerInvariant();
        return Regex.IsMatch(codigo, @"\A[ybndrfg8ejkmcpqxot1uwisza345h769]{51}[yo]\z") ? codigo : null;
    }

    internal static string Preparar(string pasta)
    {
        Directory.CreateDirectory(pasta);
        string destino = Path.Combine(pasta, "deskline.exe");
        // Uma atualização aceita no aplicativo ocupa este mesmo caminho.
        if (File.Exists(destino)) return destino;
        string temp = Path.Combine(pasta, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            using (Stream origem = typeof(Motor).Assembly.GetManifestResourceStream("deskline.exe"))
            using (var arquivo = new FileStream(temp, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
            using (var sha = SHA256.Create())
            {
                origem.CopyTo(arquivo);
                arquivo.Position = 0;
                string soma = BitConverter.ToString(sha.ComputeHash(arquivo)).Replace("-", "").ToLowerInvariant();
                if (soma != Hash) throw new InvalidDataException("A cópia incluída do Deskline está danificada.");
            }
            try { File.Move(temp, destino); }
            catch (IOException) { if (!File.Exists(destino)) throw; }
            return destino;
        }
        finally { File.Delete(temp); }
    }

    internal static void Abrir(string codigo)
    {
        string raiz = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Deskline Pessoal");
        string perfil = Path.Combine(raiz, "Perfil");
        Directory.CreateDirectory(perfil);
        string settings = Path.Combine(perfil, "settings.json");
        if (!File.Exists(settings))
        {
            string temp = Path.Combine(perfil, Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                File.WriteAllText(temp, "{\"allow_control\":false}", new UTF8Encoding(false));
                try { File.Move(temp, settings); }
                catch (IOException) { if (!File.Exists(settings)) throw; }
            }
            finally { File.Delete(temp); }
        }
        string exe = Preparar(Path.Combine(raiz, "Aplicativo"));
        var inicio = new ProcessStartInfo(exe) {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(exe),
            Arguments = "--home \"" + perfil + "\"" + (codigo == null ? "" : " --connect " + codigo)
        };
        inicio.EnvironmentVariables["DESKLINE_LANG"] = "pt-BR";
        using (Process p = Process.Start(inicio)) { }
    }
}

internal sealed class Inicio : Form
{
    readonly TextBox campo = new TextBox();
    readonly Label aviso = new Label();
    static readonly Color Tinta = Color.FromArgb(25, 43, 53);
    static readonly Color Fundo = Color.FromArgb(247, 247, 241);
    static readonly Color Verde = Color.FromArgb(22, 91, 77);

    internal Inicio()
    {
        Text = "Deskline Pessoal";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(536, 428);
        BackColor = Fundo;
        ForeColor = Tinta;
        Font = new Font("Segoe UI", 10);

        Rotulo("Deskline Pessoal", 24, 20, 24, true, 52);
        Rotulo("Receber ajuda", 26, 94, 15, true, 34);
        Rotulo("Abra seu código e envie para quem vai ajudar.", 26, 132, 10, false, 28);
        Botao("Abrir meu código", 26, 165, 190, false, delegate { Abrir(null); });

        var linha = new Panel { BackColor = Color.FromArgb(205, 215, 208) };
        linha.SetBounds(26, 228, 482, 1);
        Controls.Add(linha);
        Rotulo("Ajudar alguém", 26, 247, 15, true, 34);
        Rotulo("Cole o código do outro computador.", 26, 284, 10, false, 28);
        campo.SetBounds(26, 319, 356, 40);
        campo.Font = new Font("Consolas", 12);
        campo.AccessibleName = "Código do outro computador";
        Controls.Add(campo);
        var conectar = Botao("Conectar", 394, 315, 114, true, delegate {
            string codigo = Motor.Codigo(campo.Text);
            if (codigo == null) {
                aviso.Text = "Confira se copiou o código completo de 52 caracteres.";
                campo.Focus();
                return;
            }
            Abrir(codigo);
        });
        campo.TextChanged += delegate { aviso.Text = ""; };
        campo.KeyDown += delegate(object s, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; conectar.PerformClick(); }
        };
        aviso.SetBounds(26, 370, 482, 48);
        aviso.ForeColor = Color.Maroon;
        Controls.Add(aviso);
    }

    void Rotulo(string texto, int x, int y, int pontos, bool negrito, int altura)
    {
        var label = new Label { Text = texto, Font = new Font("Segoe UI", pontos, negrito ? FontStyle.Bold : FontStyle.Regular) };
        label.SetBounds(x, y, 485, altura);
        Controls.Add(label);
    }

    Button Botao(string texto, int x, int y, int largura, bool primario, EventHandler clique)
    {
        var btn = new Button { Text = texto, FlatStyle = FlatStyle.Flat, BackColor = primario ? Verde : Color.White, ForeColor = primario ? Color.White : Tinta };
        btn.FlatAppearance.BorderColor = Verde;
        btn.SetBounds(x, y, largura, 40);
        btn.Click += clique;
        Controls.Add(btn);
        return btn;
    }

    void Abrir(string codigo)
    {
        try { Motor.Abrir(codigo); Close(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Falha ao abrir o Deskline", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Inicio());
    }
}
