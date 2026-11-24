# Deskline Pessoal

Versão portátil para Windows 10 e 11 de 64 bits. O arquivo `dist/DesklinePessoal.exe` reúne um iniciador em português e o aplicativo oficial Deskline 0.9.0. Você pode copiar esse único arquivo para outro computador. Cada usuário recebe seu próprio código na primeira abertura do aplicativo.

## Primeira sessão

1. Nos dois computadores, abra **DesklinePessoal.exe**.
2. Quem recebe ajuda clica em **Abrir meu código** e, na janela do Deskline, em **Copiar código**. Envia o código pelo canal que já usa para conversar.
3. Quem ajuda cola o código no campo **Ajudar alguém** e clica em **Conectar**.
4. Quem recebe o pedido confere quem está chamando e clica em **Aceitar**. No perfil pessoal novo, **Deixar controlar** começa desmarcado. Marque a opção para liberar mouse e teclado.
5. Durante a sessão, a barra do Deskline permite retirar o controle ou **Parar de compartilhar**. Quem ajuda encontra arquivos, som e chat na interface da sessão.

A conexão tenta usar um caminho direto na rede local ou pela internet. O Deskline também usa serviços públicos de descoberta e pode encaminhar tráfego criptografado por um relay. Você pode configurar um relay próprio nas configurações.

## Perfil e atualizações

O iniciador extrai o aplicativo em `%LOCALAPPDATA%\Deskline Pessoal\Aplicativo`. O perfil portátil fica em `%LOCALAPPDATA%\Deskline Pessoal\Perfil` e guarda a identidade, os recentes e as preferências. O código enviado a outra pessoa é público. O arquivo de identidade do perfil é privado. A exclusão do perfil gera outro código na próxima abertura.

As atualizações do motor continuam pelo canal oficial do Deskline. O iniciador aproveita a versão já extraída nas próximas aberturas. O arquivo distribuído inclui a versão 0.9.0 e o iniciador pessoal não tem assinatura Authenticode.

Se o serviço do Deskline já estiver instalado para todos os usuários, as conexões recebidas usam a identidade e as autorizações do serviço. O iniciador pessoal abre no modo portátil e a instalação do serviço continua sendo uma escolha nas configurações.

## Origem e compilação

O motor nativo é o binário de Windows publicado em [luannzin/deskline v0.9.0](https://github.com/luannzin/deskline/releases/tag/v0.9.0). A edição pessoal acrescenta a tela de entrada, a seleção de português e o perfil portátil. O código do iniciador está em `pessoal/Pessoal.cs`.

Para gerar novamente no Windows com o compilador C# do .NET Framework, execute `pessoal/compilar.ps1` no PowerShell. O script confere o SHA-256 publicado antes de incluir o motor no executável.

Os clientes oficiais de macOS e Linux usam o mesmo protocolo e estão na [página da versão](https://github.com/luannzin/deskline/releases/tag/v0.9.0).
