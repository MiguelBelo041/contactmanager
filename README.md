ContactManager

ContactManager é uma aplicação desktop em WPF que conversa com uma API REST pra fazer um CRUD simples de contatos com nome, sobrenome e telefone. A API está hospedada no Render em container Docker, e o banco é um Azure SQL Database, também na nuvem. Pra rodar a aplicação na máquina, basta abrir o projeto WPF, ele já vem configurado pra conversar com a API hospedada, então não precisa configurar banco nem subir API.

Tecnologias

A stack é .NET 8, ASP.NET Core Web API com Swagger, Entity Framework Core 8 com o provider SqlServer, e WPF seguindo o padrão MVVM. A comunicação entre WPF e API é feita com HttpClient sobre HTTPS. O banco é Azure SQL Database no plano serverless com auto-pause, então fica em zero custo dentro dos limites grátis. A API roda em container Docker no Render (free tier), e o Dockerfile faz build em duas etapas pra deixar a imagem leve.

Como está organizado

A solution tem dois projetos. No lado do backend, o ContactManager.Api separa as responsabilidades em algumas pastas: o ContactsController fica em Controllers, a entidade Contact em Models, o AppDbContext em Data, e os DTOs de criação e atualização em DTOs. O Program.cs, o appsettings.json e o Dockerfile ficam na raiz.

Do lado do desktop, o ContactManager.Wpf segue uma divisão coerente com a mentalidade MVVM: tem o Contact em Models, o ContactApiService em Services, a MainViewModel junto com as classes base ViewModelBase e RelayCommand em ViewModels, e um ShortGuidConverter em Converters. A MainWindow.xaml e seu code-behind ficam na raiz do projeto. A MainViewModel concentra todo o estado e a lógica da tela, e o code-behind do MainWindow ficou enxuto: o InitializeComponent, um handler pra deselecionar a linha do DataGrid quando o usuário clica fora dele, e três handlers no campo Telefone que bloqueiam qualquer tecla, espaço ou paste que contenha não-dígito.

Como rodar

Precisa de Windows e do .NET 8 SDK. Como a API e o banco já estão hospedados na nuvem, não precisa configurar nada além disso.

Abre um terminal na pasta ContactManager.Wpf e roda dotnet run. A janela abre e já carrega a lista de contatos do Azure SQL através da API. Como o Render free tier suspende o container após 15 minutos de inatividade, o primeiro acesso pode demorar entre 30 e 60 segundos pra acordar o container. Os acessos seguintes são instantâneos.

O endereço da API hospedada fica no arquivo ContactManager.Wpf/appsettings.json, na chave ApiBaseUrl. Se quiser apontar pra outra instância, basta editar e recompilar.

Endpoints da API

A API expõe cinco rotas em /api/contacts. GET sem parâmetro lista todos os contatos ordenados por nome. GET com Guid busca um contato específico. POST cria, esperando um JSON com nome, sobrenome e telefone (sem Id, o servidor gera). PUT atualiza e DELETE remove, ambos recebendo o Guid na URL. Os três campos são obrigatórios, validados via DataAnnotations. O campo telefone tem uma validação adicional de RegularExpression que aceita apenas dígitos, então uma chamada com espaço ou hífen no telefone devolve 400 com a mensagem em português. A documentação interativa está disponível no Swagger, em /swagger na raiz da API.

Decisões que vale comentar

Implementei a base do MVVM na mão, com uma ViewModelBase usando INotifyPropertyChanged e um RelayCommand, em vez de puxar o CommunityToolkit.Mvvm. São poucas linhas e me ajudaram a entender melhor como o binding funciona por dentro.

Separei DTOs da entidade Contact porque o cliente nunca deveria mandar o Id, e separando fica claro que o servidor é quem gera com Guid.NewGuid. A entidade vive no banco, o DTO é o contrato com o cliente.

A connection string nunca vai pro repositório. Em produção ela fica como variável de ambiente do Render. O Program.cs falha rápido com mensagem clara se ela não estiver presente, o que evita situações estranhas de configuração errada passar despercebida.

Validei o telefone tanto no backend quanto no frontend. No backend, um RegularExpression no DTO bloqueia qualquer entrada não numérica. No frontend, três handlers no TextBox (PreviewTextInput, PreviewKeyDown e DataObject.Pasting) impedem o usuário de digitar ou colar caractere errado. A validação backend é a fonte da verdade, o frontend é só pra UX ficar mais fluida.

No DataGrid o Guid completo poluía visualmente, então criei um ShortGuidConverter que mostra só os primeiros 8 caracteres. O Guid inteiro continua disponível no tooltip ao passar o mouse, e como é um IValueConverter, a transformação é só visual, o Model continua íntegro.

Depois de cada Salvar, Atualizar e Excluir o WPF chama o GET de novo pra recarregar a lista. Custa um round-trip extra, mas garante que a tela reflete exatamente o que está no banco.

Possíveis melhorias

Trocar EnsureCreated por migrations do EF Core pra ter histórico do schema versionado, escrever testes unitários da MainViewModel mockando o ContactApiService, adicionar busca/filtro na lista, paginar o GET pra escalar com volumes maiores, e configurar HealthCheck no Render pra reduzir ou tornar visível o cold start. Em uma evolução real eu também trocaria a injeção manual do ContactApiService na MainViewModel por DI com Microsoft.Extensions.DependencyInjection no WPF, e moveria a connection string do Azure SQL pro Azure Key Vault em vez de variável de ambiente.
