using System;
using Orchard.Indexing;
using Tobe.Collection.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;

namespace Tobe.Collection {
    public class Migrations : DataMigrationImpl {
        private readonly IRepository<Agent> _agentRepository;
        private readonly IRepository<AmgRating> _amgRatingRepository; 
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Format> _formatRepository; 
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Length> _lengthRepository; 
        private readonly IRepository<RecordLabel> _recordLabelRepository;
        private readonly IRepository<Role> _roleRepository;

        public Migrations(
            IRepository<Agent> agentRepository,
            IRepository<AmgRating> amgRatingRepository,
            IRepository<Country> countryRepository,
            IRepository<Format> formatRepository,
            IRepository<Genre> genreRepository,
            IRepository<Length> lengthRepository,
            IRepository<RecordLabel> recordLabelRepository,
            IRepository<Role> roleRepository) {

            _agentRepository = agentRepository;
            _amgRatingRepository = amgRatingRepository;
            _countryRepository = countryRepository;
            _formatRepository = formatRepository;
            _genreRepository = genreRepository;
            _lengthRepository = lengthRepository;
            _recordLabelRepository = recordLabelRepository;
            _roleRepository = roleRepository;
        }

        public int Create() {
            // Agent
            SchemaBuilder.CreateTable("Agent", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                .Column<string>("NameSort", c => c.WithLength(128))
                .Column<string>("FileUnder", c => c.WithLength(128))
                );

            // Album
            SchemaBuilder.CreateTable("AlbumPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("AmgRatingId")
                .Column<int>("FormatId")
                .Column<int>("LengthId")
                .Column<string>("ArtistDisplayName", c => c.WithLength(300))
                .Column<string>("CatalogNumber", c => c.WithLength(64))
                .Column<string>("Description", c => c.WithLength(4000))
                .Column<string>("Notes", c => c.WithLength(1000))
                .Column<string>("OriginalReleaseDate", c => c.WithLength(64))
                .Column<string>("PurchasedAt", c => c.WithLength(300))
                .Column<string>("PurchaseDate", c => c.WithLength(64))
                .Column<string>("RecordedAt", c => c.WithLength(1000))
                .Column<string>("VersionReleaseDate", c => c.WithLength(64))
                .Column<bool>("ColoredVinyl")
                .Column<bool>("CoverArtPick")
                .Column<bool>("OriginalPressing")
                .Column<bool>("TobyPick")
                .Column<decimal>("PricePaid")
                );
            ContentDefinitionManager.AlterPartDefinition("AlbumPart", part => part
                .WithField("Countries", f => f.OfType("CountriesField"))
                .WithField("Genres", f => f.OfType("GenresField"))
                .WithField("RecordLabels", f => f.OfType("RecordLabelsField"))
            );

            ContentDefinitionManager.AlterTypeDefinition("Album", type => type
                .WithPart("AlbumPart")
                .WithPart("AutoroutePart", part => part
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Album Title', Pattern: 'albums/{Content.Slug}', Description: 'albums/album-title'},{Name:'Sound Recording', Pattern: 'sounds/{Content.Slug}', Description: 'sounds/sound-recording'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("CommonPart")
                .WithPart("ArtistsContainerPart")
                .WithPart("CreditsContainerPart")
                .WithPart("TitlePart")
                .Creatable()
                .Draftable()
                .Indexed("Search")
                );

            // AmgRating
            SchemaBuilder.CreateTable("AmgRating", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Artists
            SchemaBuilder.CreateTable("Artist", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContainerId")
                .Column<int>("Agent_Id")
                .Column<int>("Delimiter_Id")
                );

            // ArtistsContainerPart
            ContentDefinitionManager.AlterPartDefinition("ArtistsContainerPart", part => part
                .Attachable()
                .WithDescription("Enables your content types to store artists."));
            
            // ContentCountries
            SchemaBuilder.CreateTable("ContentCountries", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContentItemId")
                .Column<string>("FieldName", c => c.WithLength(128))
                .Column<int>("CountryId"));

            // ContentGenres
            SchemaBuilder.CreateTable("ContentGenres", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContentItemId")
                .Column<string>("FieldName", c => c.WithLength(128))
                .Column<int>("GenreId"));

            // ContentRecordLabels
            SchemaBuilder.CreateTable("ContentRecordLabels", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContentItemId")
                .Column<string>("FieldName", c => c.WithLength(128))
                .Column<int>("RecordLabelId"));

            // Country
            SchemaBuilder.CreateTable("Country", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Credits
            SchemaBuilder.CreateTable("Credit", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContainerId")
                .Column<int>("Agent_Id")
                .Column<int>("Role_Id")
                );

            // CreditsContainerPart
            ContentDefinitionManager.AlterPartDefinition("CreditsContainerPart", part => part
                .Attachable()
                .WithDescription("Enables your content types to store credits."));

            // Delimiter
            SchemaBuilder.CreateTable("Delimiter", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Format
            SchemaBuilder.CreateTable("Format", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Genre
            SchemaBuilder.CreateTable("Genre", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Length
            SchemaBuilder.CreateTable("Length", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // RecordLabel
            SchemaBuilder.CreateTable("RecordLabel", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            // Role
            SchemaBuilder.CreateTable("Role", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(128))
                );

            CreateAgents();
            CreateAmgRatings();
            CreateCountries();
            CreateFormats();
            CreateGenres();
            CreateLengths();
            CreateRecordLabels();
            CreateRoles();
            return 1;
        }

        private void CreateAgents() {
			_agentRepository.Create(new Agent { Name = "10,000 Maniacs", NameSort = "10,000 Maniacs" });
			_agentRepository.Create(new Agent { Name = "764-HERO", NameSort = "764-HERO" });
			_agentRepository.Create(new Agent { Name = "9353", NameSort = "9353" });
			_agentRepository.Create(new Agent { Name = "Aaviko", NameSort = "Aaviko" });
			_agentRepository.Create(new Agent { Name = "Abba", NameSort = "Abba" });
			_agentRepository.Create(new Agent { Name = "Abdullah Ibrahim & Ekaya", NameSort = "Abdullah Ibrahim & Ekaya" });
			_agentRepository.Create(new Agent { Name = "Abdullah Ibrahim", NameSort = "Abdullah Ibrahim" });
			_agentRepository.Create(new Agent { Name = "AC/DC", NameSort = "AC/DC" });
			_agentRepository.Create(new Agent { Name = "Adam Granduciel", NameSort = "Granduciel, Adam" });
			_agentRepository.Create(new Agent { Name = "Adam Rhodes", NameSort = "Rhodes, Adam" });
			_agentRepository.Create(new Agent { Name = "Admiral Bailey", NameSort = "Admiral Bailey" });
			_agentRepository.Create(new Agent { Name = "Aerosmith", NameSort = "Aerosmith" });
			_agentRepository.Create(new Agent { Name = "African Brothers Band", NameSort = "African Brothers Band" });
			_agentRepository.Create(new Agent { Name = "Aggrovators", NameSort = "Aggrovators" });
			_agentRepository.Create(new Agent { Name = "Air", NameSort = "Air" });
			_agentRepository.Create(new Agent { Name = "Airto Moreira", NameSort = "Moreira, Airto" });
			_agentRepository.Create(new Agent { Name = "Al Bell", NameSort = "Bell, Al" });
			_agentRepository.Create(new Agent { Name = "Al DiMeola", NameSort = "DiMeola, Al" });
			_agentRepository.Create(new Agent { Name = "Al Jackson", NameSort = "Jackson, Al" });
			_agentRepository.Create(new Agent { Name = "Al Jones", NameSort = "Jones, Al" });
			_agentRepository.Create(new Agent { Name = "Al Pachucki", NameSort = "Pachucki, Al" });
			_agentRepository.Create(new Agent { Name = "Al Schmitt", NameSort = "Schmitt, Al" });
			_agentRepository.Create(new Agent { Name = "Alan Douglas", NameSort = "Douglas, Alan" });
			_agentRepository.Create(new Agent { Name = "Alan Shacklock", NameSort = "Shacklock, Alan" });
			_agentRepository.Create(new Agent { Name = "Albert Collins", NameSort = "Collins, Albert" });
			_agentRepository.Create(new Agent { Name = "Albert King", NameSort = "King, Albert" });
			_agentRepository.Create(new Agent { Name = "Alex Chilton", NameSort = "Chilton, Alex" });
			_agentRepository.Create(new Agent { Name = "Alexander Spence", NameSort = "Spence, Alexander" });
			_agentRepository.Create(new Agent { Name = "Alfred Lion", NameSort = "Lion, Alfred" });
			_agentRepository.Create(new Agent { Name = "Alice Cooper", NameSort = "Cooper, Alice" });
			_agentRepository.Create(new Agent { Name = "Allen Jones", NameSort = "Jones, Allen" });
			_agentRepository.Create(new Agent { Name = "Allen Stanton", NameSort = "Stanton, Allen" });
			_agentRepository.Create(new Agent { Name = "Allen Toussaint", NameSort = "Toussaint, Allen" });
			_agentRepository.Create(new Agent { Name = "Allen Zentz", NameSort = "Zentz, Allen" });
			_agentRepository.Create(new Agent { Name = "Aloe Blacc", NameSort = "Blacc, Aloe" });
			_agentRepository.Create(new Agent { Name = "Althea & Donna", NameSort = "Althea & Donna" });
			_agentRepository.Create(new Agent { Name = "Alton Ellis", NameSort = "Ellis, Alton" });
			_agentRepository.Create(new Agent { Name = "Amy Winehouse", NameSort = "Winehouse, Amy" });
			_agentRepository.Create(new Agent { Name = "Andoh", NameSort = "Andoh" });
			_agentRepository.Create(new Agent { Name = "Andrés Segovia", NameSort = "Segovia, Andrés" });
			_agentRepository.Create(new Agent { Name = "Andrew Bird", NameSort = "Bird, Andrew" });
			_agentRepository.Create(new Agent { Name = "Andrew Hill", NameSort = "Hill, Andrew" });
			_agentRepository.Create(new Agent { Name = "Andrew Johns", NameSort = "Johns, Andrew" });
			_agentRepository.Create(new Agent { Name = "Andrew Ratcliffe", NameSort = "Ratcliffe, Andrew" });
			_agentRepository.Create(new Agent { Name = "Andrew Rowan Summers", NameSort = "Summers, Andrew Rowan" });
			_agentRepository.Create(new Agent { Name = "Andy Cabic", NameSort = "Cabic, Andy" });
			_agentRepository.Create(new Agent { Name = "Andy Warhol", NameSort = "Warhol, Andy" });
			_agentRepository.Create(new Agent { Name = "Animal Collective", NameSort = "Animal Collective" });
			_agentRepository.Create(new Agent { Name = "Anthony Cruz", NameSort = "Cruz, Anthony" });
			_agentRepository.Create(new Agent { Name = "Antonio Carlos Jobim", NameSort = "Jobim, Antonio Carlos" });
			_agentRepository.Create(new Agent { Name = "Archer", NameSort = "Archer" });
			_agentRepository.Create(new Agent { Name = "Archie Bell & The Drells", NameSort = "Bell & The Drells, Archie" });
			_agentRepository.Create(new Agent { Name = "Archie Shepp & The Full Moon Ensemble", NameSort = "Shepp & The Full Moon Ensemble, Archie", FileUnder = "Archie Shepp" });
			_agentRepository.Create(new Agent { Name = "Aretha Franklin", NameSort = "Franklin, Aretha" });
			_agentRepository.Create(new Agent { Name = "Arif Mardin", NameSort = "Mardin, Arif" });
			_agentRepository.Create(new Agent { Name = "Arnold Blair", NameSort = "Blair, Arnold" });
			_agentRepository.Create(new Agent { Name = "Art Blakey & The Jazz Messengers", NameSort = "Blakey & The Jazz Messengers, Art", FileUnder = "Art Blakey" });
			_agentRepository.Create(new Agent { Name = "Art Neville", NameSort = "Neville, Art" });
			_agentRepository.Create(new Agent { Name = "Art Pepper", NameSort = "Pepper, Art" });
			_agentRepository.Create(new Agent { Name = "Arthur Lyman", NameSort = "Lyman, Arthur" });
			_agentRepository.Create(new Agent { Name = "Arthur Moorhead", NameSort = "Moorhead, Arthur" });
			_agentRepository.Create(new Agent { Name = "Arthur Prysock", NameSort = "Prysock, Arthur" });
			_agentRepository.Create(new Agent { Name = "Ashley Hutchings", NameSort = "Hutchings, Ashley" });
			_agentRepository.Create(new Agent { Name = "Assagai", NameSort = "Assagai" });
			_agentRepository.Create(new Agent { Name = "Aston Barrett", NameSort = "Barrett, Aston" });
			_agentRepository.Create(new Agent { Name = "Astrud Gilberto", NameSort = "Gilberto, Astrud" });
			_agentRepository.Create(new Agent { Name = "Augustus \"Gussie\" Clarke", NameSort = "Clarke, Augustus \"Gussie\"" });
			_agentRepository.Create(new Agent { Name = "Augustus Pablo ", NameSort = "Pablo, Augustus" });
			_agentRepository.Create(new Agent { Name = "Augustus Pablo", NameSort = "Pablo, Augustus" });
			_agentRepository.Create(new Agent { Name = "Austin Godsey", NameSort = "Godsey, Austin" });
			_agentRepository.Create(new Agent { Name = "B.B. King", NameSort = "King, B.B." });
			_agentRepository.Create(new Agent { Name = "B.B. Seaton", NameSort = "Seaton, B.B." });
			_agentRepository.Create(new Agent { Name = "Baby Huey", NameSort = "Baby Huey" });
			_agentRepository.Create(new Agent { Name = "Baker Bigsby", NameSort = "Bigsby, Baker" });
			_agentRepository.Create(new Agent { Name = "Baker Bixby", NameSort = "Bigsby, Baker" });
			_agentRepository.Create(new Agent { Name = "Ballet Africains De La Republic De Guinee", NameSort = "Ballet Africains De La Republic De Guinee" });
			_agentRepository.Create(new Agent { Name = "Band of Horses", NameSort = "Band of Horses" });
			_agentRepository.Create(new Agent { Name = "Barbara Dane", NameSort = "Dane, Barbara" });
			_agentRepository.Create(new Agent { Name = "Bar-Kays", NameSort = "Bar-Kays" });
			_agentRepository.Create(new Agent { Name = "Barnabus", NameSort = "Barnabus" });
			_agentRepository.Create(new Agent { Name = "Barrington Levy", NameSort = "Levy, Barrington" });
			_agentRepository.Create(new Agent { Name = "Barry Altschul", NameSort = "Altschul, Barry" });
			_agentRepository.Create(new Agent { Name = "Barry Brown", NameSort = "Brown, Barry" });
			_agentRepository.Create(new Agent { Name = "Barry Rudolph", NameSort = "Barry Rudolph" });
			_agentRepository.Create(new Agent { Name = "Barry White & The Love Unlimited Orchestra", NameSort = "White & The Love Unlimited Orchestra, Barry", FileUnder = "Barry White" });
			_agentRepository.Create(new Agent { Name = "Beastie Boys", NameSort = "Beastie Boys" });
			_agentRepository.Create(new Agent { Name = "Beat Brigade", NameSort = "Beat Brigade" });
			_agentRepository.Create(new Agent { Name = "Beck Hansen", NameSort = "Beck Hansen", FileUnder = "Beck" });
			_agentRepository.Create(new Agent { Name = "Beck", NameSort = "Beck" });
			_agentRepository.Create(new Agent { Name = "Belle & Sebastian", NameSort = "Belle & Sebastian" });
			_agentRepository.Create(new Agent { Name = "Bernie Wyte and his Polka Band", NameSort = "Wyte and his Polka Band, Bernie" });
			_agentRepository.Create(new Agent { Name = "Bert de Coteaux", NameSort = "Coteaux, Bert de" });
			_agentRepository.Create(new Agent { Name = "Bertram Brown", NameSort = "Brown, Bertram" });
			_agentRepository.Create(new Agent { Name = "Bettie Serveert", NameSort = "Bettie Serveert" });
			_agentRepository.Create(new Agent { Name = "Betty Cantor", NameSort = "Cantor, Betty" });
			_agentRepository.Create(new Agent { Name = "Bibi Den's Tshibayi", NameSort = "Bibi Den's Tshibayi" });
			_agentRepository.Create(new Agent { Name = "Big Bill Broonzy", NameSort = "Big Bill Broonzy" });
			_agentRepository.Create(new Agent { Name = "Big Brother & The Holding Company", NameSort = "Big Brother & The Holding Company" });
			_agentRepository.Create(new Agent { Name = "Big Business", NameSort = "Big Business" });
			_agentRepository.Create(new Agent { Name = "Big John Patton", NameSort = "Big John Patton" });
			_agentRepository.Create(new Agent { Name = "Big Mama Thornton", NameSort = "Big Mama Thornton" });
			_agentRepository.Create(new Agent { Name = "Big Star", NameSort = "Big Star" });
			_agentRepository.Create(new Agent { Name = "Big Youth", NameSort = "Big Youth" });
			_agentRepository.Create(new Agent { Name = "Bill Bradley", NameSort = "Bradley, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Cosby", NameSort = "Cosby, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Doggett", NameSort = "Doggett, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Gill", NameSort = "Gill, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Halverson", NameSort = "Halverson, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Laswell", NameSort = "Laswell, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Mulvey", NameSort = "Mulvey, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Orcutt", NameSort = "Orcutt, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Porter", NameSort = "Porter, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Price", NameSort = "Price, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Randle", NameSort = "Randle, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Szymczyk", NameSort = "Szymczyk, Bill" });
			_agentRepository.Create(new Agent { Name = "Bill Wolf", NameSort = "Wolf, Bill" });
			_agentRepository.Create(new Agent { Name = "Billy Bragg", NameSort = "Bragg, Billy" });
			_agentRepository.Create(new Agent { Name = "Billy Cobham", NameSort = "Cobham, Billy" });
			_agentRepository.Create(new Agent { Name = "Billy Joel", NameSort = "Joel, Billy" });
			_agentRepository.Create(new Agent { Name = "Billy Wolf", NameSort = "Wolf, Billy" });
			_agentRepository.Create(new Agent { Name = "Bis", NameSort = "Bis" });
			_agentRepository.Create(new Agent { Name = "Black Flag", NameSort = "Black Flag" });
			_agentRepository.Create(new Agent { Name = "Black Sabbath", NameSort = "Black Sabbath" });
			_agentRepository.Create(new Agent { Name = "Black Uhuru", NameSort = "Black Uhuru" });
			_agentRepository.Create(new Agent { Name = "Blind Faith", NameSort = "Blind Faith" });
			_agentRepository.Create(new Agent { Name = "BLK JKS", NameSort = "BLK JKS" });
			_agentRepository.Create(new Agent { Name = "Blondie", NameSort = "Blondie" });
			_agentRepository.Create(new Agent { Name = "Blood Sweat & Tears", NameSort = "Blood Sweat & Tears" });
			_agentRepository.Create(new Agent { Name = "Bloodrock", NameSort = "Bloodrock" });
			_agentRepository.Create(new Agent { Name = "Blues Brothers", NameSort = "Blues Brothers" });
			_agentRepository.Create(new Agent { Name = "Bo Diddley", NameSort = "Diddley, Bo" });
			_agentRepository.Create(new Agent { Name = "Bob Both", NameSort = "Both, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Cantor", NameSort = "Cantor, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Clearmountain", NameSort = "Clearmountain, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Cole", NameSort = "Cole, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Cummins", NameSort = "Cummins, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob d'Orleans", NameSort = "d'Orleans, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Dylan and the Band", NameSort = "Dylan and the Band, Bob", FileUnder = "Bob Dylan" });
			_agentRepository.Create(new Agent { Name = "Bob Dylan", NameSort = "Dylan, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Hughes", NameSort = "Hughes, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Johnston", NameSort = "Johnston, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Liftin", NameSort = "Liftin, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Marley & The Wailers", NameSort = "Marley & The Wailers, Bob", FileUnder = "Bob Marley" });
			_agentRepository.Create(new Agent { Name = "Bob Marley", NameSort = "Marley, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Mould", NameSort = "Mould, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Porter", NameSort = "Porter, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Rolontz", NameSort = "Rolontz, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Sargent", NameSort = "Sargent, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Schoenfield", NameSort = "Schoenfield, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Shumaker", NameSort = "Shumaker, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Simpson", NameSort = "Simpson, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Sullivan", NameSort = "Sullivan, Bob" });
			_agentRepository.Create(new Agent { Name = "Bob Thiele", NameSort = "Thiele, Bob" });
			_agentRepository.Create(new Agent { Name = "Bobby Bloom", NameSort = "Bloom, Bobby" });
			_agentRepository.Create(new Agent { Name = "Bobby Harlow", NameSort = "Harlow, Bobby" });
			_agentRepository.Create(new Agent { Name = "Bobby Kalphat", NameSort = "Kalphat, Bobby" });
			_agentRepository.Create(new Agent { Name = "Bobby Previte", NameSort = "Previte, Bobby" });
			_agentRepository.Create(new Agent { Name = "Bongo Gene All Stars", NameSort = "Bongo Gene All Stars", FileUnder = "Bongo Gene Campbell" });
			_agentRepository.Create(new Agent { Name = "Bongo Gene Campbell", NameSort = "Campbell, Bongo Gene" });
			_agentRepository.Create(new Agent { Name = "Bongo Herman", NameSort = "Bongo Herman" });
			_agentRepository.Create(new Agent { Name = "Bongwater", NameSort = "Bongwater" });
			_agentRepository.Create(new Agent { Name = "Bonnie \"Prince\" Billy", NameSort = "Bonnie \"Prince\" Billy", FileUnder = "Will Oldham" });
			_agentRepository.Create(new Agent { Name = "Bonnie Raitt", NameSort = "Raitt, Bonnie" });
			_agentRepository.Create(new Agent { Name = "Booker Ervin", NameSort = "Ervin, Booker" });
			_agentRepository.Create(new Agent { Name = "Booker T & The MG's", NameSort = "Booker T & The MG's" });
			_agentRepository.Create(new Agent { Name = "Booker T. & the MG's", NameSort = "Booker T. & the MG's" });
			_agentRepository.Create(new Agent { Name = "Bootsy Collins", NameSort = "Collins, Bootsy" });
			_agentRepository.Create(new Agent { Name = "Boris", NameSort = "Boris" });
			_agentRepository.Create(new Agent { Name = "Borris Gardiner", NameSort = "Gardiner, Borris" });
			_agentRepository.Create(new Agent { Name = "Boubacar Traore", NameSort = "Traore, Boubacar" });
			_agentRepository.Create(new Agent { Name = "Brad Osborne", NameSort = "Osborne, Brad" });
			_agentRepository.Create(new Agent { Name = "Bradley Cook", NameSort = "Cook, Bradley" });
			_agentRepository.Create(new Agent { Name = "Brandon Curtis", NameSort = "Curtis, Brandon" });
			_agentRepository.Create(new Agent { Name = "Brent Lambert", NameSort = "Lambert, Brent" });
			_agentRepository.Create(new Agent { Name = "Brian Ahern", NameSort = "Ahern, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Eno", NameSort = "Eno, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Gaynor", NameSort = "Gaynor, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Humphries", NameSort = "Humphries, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Jenkins", NameSort = "Jenkins, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Ross", NameSort = "Ross, Brian" });
			_agentRepository.Create(new Agent { Name = "Brian Ross-Myring", NameSort = "Ross-Myring, Brian" });
			_agentRepository.Create(new Agent { Name = "Brigitte Bardot", NameSort = "Bardot, Brigitte" });
			_agentRepository.Create(new Agent { Name = "Brother Jack McDuff", NameSort = "McDuff, Brother Jack" });
			_agentRepository.Create(new Agent { Name = "Bruce Iglauer", NameSort = "Iglauer, Bruce" });
			_agentRepository.Create(new Agent { Name = "Bruce Springsteen", NameSort = "Springsteen, Bruce" });
			_agentRepository.Create(new Agent { Name = "Brute Force Steel Band", NameSort = "Brute Force Steel Band" });
			_agentRepository.Create(new Agent { Name = "Bryan & Tony Gold", NameSort = "Gold, Bryan & Tony" });
			_agentRepository.Create(new Agent { Name = "Bryce Goggin", NameSort = "Goggin, Bryce" });
			_agentRepository.Create(new Agent { Name = "Bryon Smith", NameSort = "Smith, Bryon" });
			_agentRepository.Create(new Agent { Name = "Buck Owens", NameSort = "Owens, Buck" });
			_agentRepository.Create(new Agent { Name = "Buckwheat Zydeco", NameSort = "Buckwheat Zydeco" });
			_agentRepository.Create(new Agent { Name = "Bud & Travis", NameSort = "Bud & Travis" });
			_agentRepository.Create(new Agent { Name = "Buddy Guy", NameSort = "Guy, Buddy" });
			_agentRepository.Create(new Agent { Name = "Built to Spill", NameSort = "Built to Spill" });
			_agentRepository.Create(new Agent { Name = "Bulby", NameSort = "Bulby" });
			_agentRepository.Create(new Agent { Name = "Bunny \"Striker\" Lee", NameSort = "Lee, Bunny \"Striker\"", FileUnder = "Bunny Lee" });
			_agentRepository.Create(new Agent { Name = "Bunny Lee", NameSort = "Lee, Bunny" });
			_agentRepository.Create(new Agent { Name = "Burl Ives", NameSort = "Ives, Burl" });
			_agentRepository.Create(new Agent { Name = "Burning Spear", NameSort = "Burning Spear" });
			_agentRepository.Create(new Agent { Name = "Burt Bacharach", NameSort = "Bacharach, Burt" });
			_agentRepository.Create(new Agent { Name = "Butch Morris", NameSort = "Morris, Butch" });
			_agentRepository.Create(new Agent { Name = "Butthole Surfers", NameSort = "Butthole Surfers" });
			_agentRepository.Create(new Agent { Name = "C.K. Mann", NameSort = "Mann, C.K." });
			_agentRepository.Create(new Agent { Name = "C.S. Dodd", NameSort = "Dodd, C.S. ", FileUnder = "Coxsone Dodd" });
			_agentRepository.Create(new Agent { Name = "Caetano Veloso", NameSort = "Veloso, Caetano" });
			_agentRepository.Create(new Agent { Name = "Cahalen Morrison & Country Hammer", NameSort = "Morrison & Country Hammer, Cahalen", FileUnder = "Cahalen Morrison" });
			_agentRepository.Create(new Agent { Name = "Cahalen Morrison", NameSort = "Morrison, Cahalen" });
			_agentRepository.Create(new Agent { Name = "Calexico", NameSort = "Calexico" });
			_agentRepository.Create(new Agent { Name = "Calvin Johnson", NameSort = "Johnson, Calvin" });
			_agentRepository.Create(new Agent { Name = "Camper Van Beethoven", NameSort = "Camper Van Beethoven" });
			_agentRepository.Create(new Agent { Name = "Cannonball Adderley", NameSort = "Cannonball Adderley" });
			_agentRepository.Create(new Agent { Name = "Capleton", NameSort = "Capleton" });
			_agentRepository.Create(new Agent { Name = "Captain Beefheart", NameSort = "Captain Beefheart" });
			_agentRepository.Create(new Agent { Name = "Carl Davis", NameSort = "Davis, Carl" });
			_agentRepository.Create(new Agent { Name = "Carl Dehaney", NameSort = "Dehaney, Carl" });
			_agentRepository.Create(new Agent { Name = "Carl Malcom", NameSort = "Malcom, Carl" });
			_agentRepository.Create(new Agent { Name = "Carlos Albrecht", NameSort = "Albrecht, Carlos" });
			_agentRepository.Create(new Agent { Name = "Carlos Montoya", NameSort = "Montoya, Carlos" });
			_agentRepository.Create(new Agent { Name = "Carlos Santana", NameSort = "Santana, Carlos" });
			_agentRepository.Create(new Agent { Name = "Carlton & The Shoes", NameSort = "Carlton & The Shoes" });
			_agentRepository.Create(new Agent { Name = "Carmen Cavallaro", NameSort = "Cavallaro, Carmen" });
			_agentRepository.Create(new Agent { Name = "Carmine Rubino", NameSort = "Rubino, Carmine" });
			_agentRepository.Create(new Agent { Name = "Carolina Chocolate Drops", NameSort = "Carolina Chocolate Drops" });
			_agentRepository.Create(new Agent { Name = "Casey Rice", NameSort = "Rice, Casey" });
			_agentRepository.Create(new Agent { Name = "Cat Power", NameSort = "Cat Power" });
			_agentRepository.Create(new Agent { Name = "Cat Stevens", NameSort = "Cat Stevens" });
			_agentRepository.Create(new Agent { Name = "Cecil McBee Sextet", NameSort = "McBee Sextet, Cecil", FileUnder = "Cecil McBee" });
			_agentRepository.Create(new Agent { Name = "Cecil McBee", NameSort = "McBee, Cecil" });
			_agentRepository.Create(new Agent { Name = "Cedric Im Brooks & The Light Of Saba", NameSort = "Brooks & The Light Of Saba, Cedric Im", FileUnder = "Cedric Im Brooks" });
			_agentRepository.Create(new Agent { Name = "Cedric Im Brooks", NameSort = "Brooks, Cedric Im" });
			_agentRepository.Create(new Agent { Name = "Cee Lo Green", NameSort = "Green, Cee Lo" });
			_agentRepository.Create(new Agent { Name = "Chad Taylor", NameSort = "Taylor, Chad" });
			_agentRepository.Create(new Agent { Name = "Charles Davis", NameSort = "Davis, Charles" });
			_agentRepository.Create(new Agent { Name = "Charles Duvelle", NameSort = "Duvelle, Charles" });
			_agentRepository.Create(new Agent { Name = "Charles Earland", NameSort = "Earland, Charles" });
			_agentRepository.Create(new Agent { Name = "Charles Mingus", NameSort = "Mingus, Charles" });
			_agentRepository.Create(new Agent { Name = "Charles R. Freeland", NameSort = "Freeland, Charles R." });
			_agentRepository.Create(new Agent { Name = "Charlie Parker", NameSort = "Parker, Charlie" });
			_agentRepository.Create(new Agent { Name = "Chas Chandler", NameSort = "Chandler, Chas" });
			_agentRepository.Create(new Agent { Name = "Cheech & Chong", NameSort = "Cheech & Chong" });
			_agentRepository.Create(new Agent { Name = "Chequered Past", NameSort = "Chequered Past" });
			_agentRepository.Create(new Agent { Name = "Chet Atkins", NameSort = "Atkins, Chet" });
			_agentRepository.Create(new Agent { Name = "Chet Parker", NameSort = "Parker, Chet" });
			_agentRepository.Create(new Agent { Name = "Chicago Steve", NameSort = "Chicago Steve" });
			_agentRepository.Create(new Agent { Name = "Chicha Libre", NameSort = "Chicha Libre" });
			_agentRepository.Create(new Agent { Name = "Chick Corea", NameSort = "Corea, Chick" });
			_agentRepository.Create(new Agent { Name = "Chico Buarque", NameSort = "Buarque, Chico" });
			_agentRepository.Create(new Agent { Name = "Chris Blackwell", NameSort = "Blackwell, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Burgess", NameSort = "Burgess, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Cutler", NameSort = "Cutler, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Huston", NameSort = "Huston, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Kirkley", NameSort = "Kirkley, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Porter", NameSort = "Porter, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Schultz", NameSort = "Schultz, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Strachwitz", NameSort = "Strachwitz, Chris" });
			_agentRepository.Create(new Agent { Name = "Chris Thomas", NameSort = "Thomas, Chris" });
			_agentRepository.Create(new Agent { Name = "Chuck Berry", NameSort = "Berry, Chuck" });
			_agentRepository.Create(new Agent { Name = "Chuck Kirkpatrick", NameSort = "Kirkpatrick, Chuck" });
			_agentRepository.Create(new Agent { Name = "Ciccone Youth", NameSort = "Ciccone Youth" });
			_agentRepository.Create(new Agent { Name = "Clay Jones", NameSort = "Jones, Clay" });
			_agentRepository.Create(new Agent { Name = "Clay Rose", NameSort = "Rose, Clay" });
			_agentRepository.Create(new Agent { Name = "Clement Dodd", NameSort = "Dodd, Clement", FileUnder = "Coxsone Dodd" });
			_agentRepository.Create(new Agent { Name = "Clifford Jordan", NameSort = "Jordan, Clifford" });
			_agentRepository.Create(new Agent { Name = "Clifton Chenier", NameSort = "Chenier, Clifton" });
			_agentRepository.Create(new Agent { Name = "Close Lobsters", NameSort = "Close Lobsters" });
			_agentRepository.Create(new Agent { Name = "Coco Tea", NameSort = "Coco Tea" });
			_agentRepository.Create(new Agent { Name = "Cocteau Twins", NameSort = "Cocteau Twins" });
			_agentRepository.Create(new Agent { Name = "Coke", NameSort = "Coke" });
			_agentRepository.Create(new Agent { Name = "Colin Roach", NameSort = "Roach, Colin" });
			_agentRepository.Create(new Agent { Name = "Come", NameSort = "Come" });
			_agentRepository.Create(new Agent { Name = "Conrad Plank", NameSort = "Plank, Conrad" });
			_agentRepository.Create(new Agent { Name = "Conway Twitty", NameSort = "Twitty, Conway" });
			_agentRepository.Create(new Agent { Name = "Cornell Campbell", NameSort = "Campbell, Cornell" });
			_agentRepository.Create(new Agent { Name = "Count Basie and his Orchestra", NameSort = "Basie and his Orchestra, Count", FileUnder = "Count Basie" });
			_agentRepository.Create(new Agent { Name = "Count Basie Orchestra", NameSort = "Basie Orchestra, Count", FileUnder = "Count Basie" });
			_agentRepository.Create(new Agent { Name = "Count Basie", NameSort = "Basie, Count" });
			_agentRepository.Create(new Agent { Name = "Count Basie", NameSort = "Count Basie" });
			_agentRepository.Create(new Agent { Name = "Count Machukie", NameSort = "Count Machukie" });
			_agentRepository.Create(new Agent { Name = "Count Shelly", NameSort = "Count Shelly" });
			_agentRepository.Create(new Agent { Name = "Cowboy Junkies", NameSort = "Cowboy Junkies" });
			_agentRepository.Create(new Agent { Name = "Cracker Jack & Patches", NameSort = "Cracker Jack & Patches" });
			_agentRepository.Create(new Agent { Name = "Cravo E Canela", NameSort = "Cravo E Canela" });
			_agentRepository.Create(new Agent { Name = "Crazy Horse", NameSort = "Crazy Horse" });
			_agentRepository.Create(new Agent { Name = "Cream", NameSort = "Cream" });
			_agentRepository.Create(new Agent { Name = "Creed Taylor", NameSort = "Taylor, Creed" });
			_agentRepository.Create(new Agent { Name = "Creedence Clearwater Revival", NameSort = "Creedence Clearwater Revival" });
			_agentRepository.Create(new Agent { Name = "Crosby", NameSort = "Crosby", FileUnder = "David Crosby" });
			_agentRepository.Create(new Agent { Name = "Crosby, Stills, Nash & Young", NameSort = "Crosby, Stills, Nash & Young" });
			_agentRepository.Create(new Agent { Name = "Curlew", NameSort = "Curlew" });
			_agentRepository.Create(new Agent { Name = "Curtis Mayfield", NameSort = "Mayfield, Curtis" });
			_agentRepository.Create(new Agent { Name = "D.B. Cooper", NameSort = "Cooper, D.B." });
			_agentRepository.Create(new Agent { Name = "D.K. Nyarko and Sons", NameSort = "Nyarko and Sons, D.K." });
			_agentRepository.Create(new Agent { Name = "D.L. Menard", NameSort = "Menard, D.L." });
			_agentRepository.Create(new Agent { Name = "Dahaud & Jean Shaar", NameSort = "Dahaud & Jean Shaar" });
			_agentRepository.Create(new Agent { Name = "Damon & Naomi", NameSort = "Damon & Naomi" });
			_agentRepository.Create(new Agent { Name = "Damon Lyon-Shaw", NameSort = "Lyon-Shaw, Damon" });
			_agentRepository.Create(new Agent { Name = "Dan Clarke", NameSort = "Clarke, Dan" });
			_agentRepository.Create(new Agent { Name = "Dan Frome", NameSort = "Frome, Dan" });
			_agentRepository.Create(new Agent { Name = "Dan Healy", NameSort = "Healy, Dan" });
			_agentRepository.Create(new Agent { Name = "Darker My Love", NameSort = "Darker My Love" });
			_agentRepository.Create(new Agent { Name = "Das Damen", NameSort = "Das Damen" });
			_agentRepository.Create(new Agent { Name = "Dave \"Baby\" Cortez", NameSort = "Cortez, Dave \"Baby\" " });
			_agentRepository.Create(new Agent { Name = "Dave Carbona", NameSort = "Carbona, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Crawford", NameSort = "Crawford, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Edmunds", NameSort = "Edmunds, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Fridmann", NameSort = "Fridmann, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Greene", NameSort = "Greene, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Hutchins", NameSort = "Hutchins, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Jerden", NameSort = "Jerden, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Kalmbach", NameSort = "Kalmbach, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Pell", NameSort = "Pell, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Powell", NameSort = "Powell, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Stevens", NameSort = "Stevens, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Trumfio", NameSort = "Trumfio, Dave" });
			_agentRepository.Create(new Agent { Name = "Dave Watson", NameSort = "Watson, Dave" });
			_agentRepository.Create(new Agent { Name = "David Axelrod", NameSort = "Axelrod, David" });
			_agentRepository.Create(new Agent { Name = "David Baker", NameSort = "Baker, David" });
			_agentRepository.Create(new Agent { Name = "David Bowie", NameSort = "Bowie, David" });
			_agentRepository.Create(new Agent { Name = "David Briggs", NameSort = "Briggs, David" });
			_agentRepository.Create(new Agent { Name = "David Byrne", NameSort = "Byrne, David" });
			_agentRepository.Create(new Agent { Name = "David Crosby", NameSort = "Crosby, David" });
			_agentRepository.Create(new Agent { Name = "David Grisman", NameSort = "Grisman, David" });
			_agentRepository.Create(new Agent { Name = "David Hassinger", NameSort = "Hassinger, David" });
			_agentRepository.Create(new Agent { Name = "David Hentschel", NameSort = "Hentschel, David" });
			_agentRepository.Create(new Agent { Name = "David Hitchcock", NameSort = "Hitchcock, David" });
			_agentRepository.Create(new Agent { Name = "David Holland", NameSort = "Holland, David", FileUnder = "Dave Holland" });
			_agentRepository.Create(new Agent { Name = "David Lord", NameSort = "Lord, David" });
			_agentRepository.Create(new Agent { Name = "David Rubinson & Friends", NameSort = "Rubinson & Friends, David", FileUnder = "David Rubinson" });
			_agentRepository.Create(new Agent { Name = "David Rubinson", NameSort = "Rubinson, David" });
			_agentRepository.Create(new Agent { Name = "David Sewelson & The 25 O'Clock Band", NameSort = "Sewelson & The 25 O'Clock Band, David", FileUnder = "David Sewelson" });
			_agentRepository.Create(new Agent { Name = "David Sylvian", NameSort = "Sylvian, David" });
			_agentRepository.Create(new Agent { Name = "David Thomas", NameSort = "Thomas, David" });
			_agentRepository.Create(new Agent { Name = "Davis McCain", NameSort = "McCain, Davis" });
			_agentRepository.Create(new Agent { Name = "Dean Fraser", NameSort = "Fraser, Dean" });
			_agentRepository.Create(new Agent { Name = "Dean Hightower", NameSort = "Hightower, Dean" });
			_agentRepository.Create(new Agent { Name = "Dean Roumanis", NameSort = "Roumanis, Dean" });
			_agentRepository.Create(new Agent { Name = "Death of Samantha", NameSort = "Death of Samantha" });
			_agentRepository.Create(new Agent { Name = "Debo Band", NameSort = "Debo Band" });
			_agentRepository.Create(new Agent { Name = "Dee-Lite", NameSort = "Dee-Lite" });
			_agentRepository.Create(new Agent { Name = "Deep Purple", NameSort = "Deep Purple" });
			_agentRepository.Create(new Agent { Name = "Delroy Wright", NameSort = "Wright, Delroy" });
			_agentRepository.Create(new Agent { Name = "Dengue Fever", NameSort = "Dengue Fever" });
			_agentRepository.Create(new Agent { Name = "Dennis (Blackbeard) Bovell", NameSort = "Bovell, Dennis (Blackbeard)", FileUnder = "Dennis Bovell" });
			_agentRepository.Create(new Agent { Name = "Dennis Bovell", NameSort = "Bovell, Dennis" });
			_agentRepository.Create(new Agent { Name = "Dennis Brown", NameSort = "Brown, Dennis" });
			_agentRepository.Create(new Agent { Name = "Dennis Herring", NameSort = "Herring, Dennis" });
			_agentRepository.Create(new Agent { Name = "Denny Cordell", NameSort = "Cordell, Denny" });
			_agentRepository.Create(new Agent { Name = "Deodato", NameSort = "Deodato" });
			_agentRepository.Create(new Agent { Name = "Derek and the Dominos", NameSort = "Derek and the Dominos" });
			_agentRepository.Create(new Agent { Name = "Devadip Carlos Santana", NameSort = "Devadip Carlos Santana" });
			_agentRepository.Create(new Agent { Name = "Devo", NameSort = "Devo" });
			_agentRepository.Create(new Agent { Name = "Devon \"Element\" Wheatley", NameSort = "Wheatley, Devon \"Element\"" });
			_agentRepository.Create(new Agent { Name = "Dewey Balfa", NameSort = "Balfa, Dewey" });
			_agentRepository.Create(new Agent { Name = "Dexter Gordon", NameSort = "Gordon, Dexter" });
			_agentRepository.Create(new Agent { Name = "Diblo Dibala", NameSort = "Dibala, Diblo" });
			_agentRepository.Create(new Agent { Name = "Diblo et Matchatcha", NameSort = "Diblo et Matchatcha", FileUnder = "Diblo Dibala" });
			_agentRepository.Create(new Agent { Name = "Dick Liebert", NameSort = "Liebert, Dick" });
			_agentRepository.Create(new Agent { Name = "Dick Marta", NameSort = "Marta, Dick" });
			_agentRepository.Create(new Agent { Name = "Diga Rhythm Band", NameSort = "Diga Rhythm Band" });
			_agentRepository.Create(new Agent { Name = "Dinosaur Jr.", NameSort = "Dinosaur Jr." });
			_agentRepository.Create(new Agent { Name = "Dinosaur", NameSort = "Dinosaur", FileUnder = "Dinosaur Jr." });
			_agentRepository.Create(new Agent { Name = "Dirty Three", NameSort = "Dirty Three" });
			_agentRepository.Create(new Agent { Name = "Disco-Tex & The Sex-O-Lettes", NameSort = "Disco-Tex & The Sex-O-Lettes" });
			_agentRepository.Create(new Agent { Name = "Django Reinhardt", NameSort = "Reinhardt, Django" });
			_agentRepository.Create(new Agent { Name = "Dntel", NameSort = "Dntel" });
			_agentRepository.Create(new Agent { Name = "Dollar Brand Duo", NameSort = "Dollar Brand Duo", FileUnder = "Abdullah Ibrahim" });
			_agentRepository.Create(new Agent { Name = "Dollar Brand", NameSort = "Dollar Brand" });
			_agentRepository.Create(new Agent { Name = "Dollar Brand", NameSort = "Dollar Brand", FileUnder = "Abdullah Ibrahim" });
			_agentRepository.Create(new Agent { Name = "Domenico Modugno", NameSort = "Modugno, Domenico" });
			_agentRepository.Create(new Agent { Name = "Don Drummond", NameSort = "Drummond, Don" });
			_agentRepository.Create(new Agent { Name = "Don Gant", NameSort = "Gant, Don" });
			_agentRepository.Create(new Agent { Name = "Don Gehman", NameSort = "Gehman, Don" });
			_agentRepository.Create(new Agent { Name = "Don Hahn", NameSort = "Hahn, Don" });
			_agentRepository.Create(new Agent { Name = "Don Law", NameSort = "Law, Don" });
			_agentRepository.Create(new Agent { Name = "Don Nix", NameSort = "Nix, Don" });
			_agentRepository.Create(new Agent { Name = "Don Patterson", NameSort = "Patterson, Don" });
			_agentRepository.Create(new Agent { Name = "Don Puluse", NameSort = "Puluse, Don" });
			_agentRepository.Create(new Agent { Name = "Don Schlitten", NameSort = "Schlitten, Don" });
			_agentRepository.Create(new Agent { Name = "Donald Byrd & 125th Street, NYC", NameSort = "Byrd & 125th Street, NYC, Donald", FileUnder = "Donald Byrd" });
			_agentRepository.Create(new Agent { Name = "Donald Byrd", NameSort = "Byrd, Donald" });
			_agentRepository.Create(new Agent { Name = "Donovan", NameSort = "Donovan" });
			_agentRepository.Create(new Agent { Name = "Dope Body", NameSort = "Dope Body" });
			_agentRepository.Create(new Agent { Name = "Doug Easley", NameSort = "Easley, Doug" });
			_agentRepository.Create(new Agent { Name = "Doug Martsch", NameSort = "Martsch, Doug" });
			_agentRepository.Create(new Agent { Name = "Doug Sahm", NameSort = "Sahm, Doug" });
			_agentRepository.Create(new Agent { Name = "Doviak", NameSort = "Doviak" });
			_agentRepository.Create(new Agent { Name = "Doyle E. Jones", NameSort = "Jones, Doyle E." });
			_agentRepository.Create(new Agent { Name = "Dr. Alimantado", NameSort = "Dr. Alimantado" });
			_agentRepository.Create(new Agent { Name = "Dr. Dre", NameSort = "Dr. Dre" });
			_agentRepository.Create(new Agent { Name = "Dr. John", NameSort = "Dr. John" });
			_agentRepository.Create(new Agent { Name = "Dr. Nico", NameSort = "Dr. Nico" });
			_agentRepository.Create(new Agent { Name = "Dream Power", NameSort = "Dream Power" });
			_agentRepository.Create(new Agent { Name = "Duane Allman", NameSort = "Allman, Duane" });
			_agentRepository.Create(new Agent { Name = "Dub Narcotic Sound System", NameSort = "Dub Narcotic Sound System" });
			_agentRepository.Create(new Agent { Name = "Dub Specialist", NameSort = "Dub Specialist" });
			_agentRepository.Create(new Agent { Name = "Dudu Pukwana", NameSort = "Pukwana, Dudu" });
			_agentRepository.Create(new Agent { Name = "Duke Ellington & His Orchestra", NameSort = "Ellington & His Orchestra, Duke", FileUnder = "Duke Ellington" });
			_agentRepository.Create(new Agent { Name = "Duke Ellington", NameSort = "Ellington, Duke" });
			_agentRepository.Create(new Agent { Name = "Duke Pearson", NameSort = "Pearson, Duke" });
			_agentRepository.Create(new Agent { Name = "Dumptruck", NameSort = "Dumptruck" });
			_agentRepository.Create(new Agent { Name = "Dust Brothers", NameSort = "Dust Brothers" });
			_agentRepository.Create(new Agent { Name = "Duster", NameSort = "Duster" });
			_agentRepository.Create(new Agent { Name = "Eagles", NameSort = "Eagles" });
			_agentRepository.Create(new Agent { Name = "Earl \"Chinna\" Smith", NameSort = "Smith, Earl \"Chinna\"" });
			_agentRepository.Create(new Agent { Name = "Earl Chin", NameSort = "Chin, Earl" });
			_agentRepository.Create(new Agent { Name = "Earl Cunningham", NameSort = "Cunningham, Earl" });
			_agentRepository.Create(new Agent { Name = "Earl Sixteen", NameSort = "Earl Sixteen" });
			_agentRepository.Create(new Agent { Name = "Earle Mankey", NameSort = "Mankey, Earle" });
			_agentRepository.Create(new Agent { Name = "Ebenezer Obey", NameSort = "Obey, Ebenezer" });
			_agentRepository.Create(new Agent { Name = "Echo and the Bunnymen", NameSort = "Echo and the Bunnymen" });
			_agentRepository.Create(new Agent { Name = "Ed Barton", NameSort = "Barton, Ed" });
			_agentRepository.Create(new Agent { Name = "Ed Lincoln", NameSort = "Lincoln, Ed" });
			_agentRepository.Create(new Agent { Name = "Ed Michel", NameSort = "Michel, Ed" });
			_agentRepository.Create(new Agent { Name = "Ed Stasium", NameSort = "Stasium, Ed" });
			_agentRepository.Create(new Agent { Name = "Eddie \"Cleanhead\" Vinson", NameSort = "Vinson, Eddie \"Cleanhead\"" });
			_agentRepository.Create(new Agent { Name = "Eddie and Gene", NameSort = "Eddie and Gene" });
			_agentRepository.Create(new Agent { Name = "Eddie Harris", NameSort = "Harris, Eddie" });
			_agentRepository.Create(new Agent { Name = "Eddie Hazel", NameSort = "Hazel, Eddie" });
			_agentRepository.Create(new Agent { Name = "Eddie Korvin", NameSort = "Korvin, Eddie" });
			_agentRepository.Create(new Agent { Name = "Eddie Kramer", NameSort = "Kramer, Eddie" });
			_agentRepository.Create(new Agent { Name = "Eddie Palmieri II", NameSort = "Palmieri II, Eddie" });
			_agentRepository.Create(new Agent { Name = "Eddie Palmieri", NameSort = "Palmieri, Eddie" });
			_agentRepository.Create(new Agent { Name = "Edward \"Bunny\" Lee aka \"Striker\"", NameSort = "Lee aka \"Striker\", Edward \"Bunny\" ", FileUnder = "Bunny Lee" });
			_agentRepository.Create(new Agent { Name = "Edward Stasiun", NameSort = "Stasiun, Edward" });
			_agentRepository.Create(new Agent { Name = "Edward Vito", NameSort = "Vito, Edward" });
			_agentRepository.Create(new Agent { Name = "Edwin H. Kramer", NameSort = "Kramer, Edwin H." });
			_agentRepository.Create(new Agent { Name = "Elis Regina", NameSort = "Regina, Elis" });
			_agentRepository.Create(new Agent { Name = "Ella Jenkins", NameSort = "Jenkins, Ella" });
			_agentRepository.Create(new Agent { Name = "Elliott Smith", NameSort = "Smith, Elliott" });
			_agentRepository.Create(new Agent { Name = "Elmore James", NameSort = "James, Elmore" });
			_agentRepository.Create(new Agent { Name = "Elton John", NameSort = "John, Elton" });
			_agentRepository.Create(new Agent { Name = "Elvin Campbell", NameSort = "Campbell, Elvin" });
			_agentRepository.Create(new Agent { Name = "Elvin Jones", NameSort = "Jones, Elvin" });
			_agentRepository.Create(new Agent { Name = "Elvis Costello & The Attractions", NameSort = "Costello & The Attractions, Elvis", FileUnder = "Elvis Costello" });
			_agentRepository.Create(new Agent { Name = "Elvis Costello", NameSort = "Costello, Elvis" });
			_agentRepository.Create(new Agent { Name = "Emerson, Lake & Palmer", NameSort = "Emerson, Lake & Palmer" });
			_agentRepository.Create(new Agent { Name = "Emile Valognes", NameSort = "Valognes, Emile" });
			_agentRepository.Create(new Agent { Name = "Emmanuel Odenusi", NameSort = "Odenusi, Emmanuel" });
			_agentRepository.Create(new Agent { Name = "Emmylou Harris", NameSort = "Harris, Emmylou" });
			_agentRepository.Create(new Agent { Name = "Emory Cook", NameSort = "Cook, Emory" });
			_agentRepository.Create(new Agent { Name = "Eno '76", NameSort = "Eno '76", FileUnder = "Brian Eno" });
			_agentRepository.Create(new Agent { Name = "Eric Dolphy", NameSort = "Dolphy, Eric" });
			_agentRepository.Create(new Agent { Name = "Eric E.T. Thorngren", NameSort = "Thorngren, Eric E.T." });
			_agentRepository.Create(new Agent { Name = "Eric Swanson", NameSort = "Swanson, Eric" });
			_agentRepository.Create(new Agent { Name = "Ernest Hoo Kim", NameSort = "Kim, Ernest Hoo" });
			_agentRepository.Create(new Agent { Name = "Ernest Ranglin", NameSort = "Ranglin, Ernest" });
			_agentRepository.Create(new Agent { Name = "Errol Brown", NameSort = "Brown, Errol" });
			_agentRepository.Create(new Agent { Name = "Errol Dunkley", NameSort = "Dunkley, Errol" });
			_agentRepository.Create(new Agent { Name = "Errol Ross", NameSort = "Ross, Errol" });
			_agentRepository.Create(new Agent { Name = "Errol T", NameSort = "Errol T", FileUnder = "Errol Thompson" });
			_agentRepository.Create(new Agent { Name = "Errol Thompson", NameSort = "Thompson, Errol" });
			_agentRepository.Create(new Agent { Name = "Erroll Garner", NameSort = "Garner, Erroll" });
			_agentRepository.Create(new Agent { Name = "Esmond Edwards", NameSort = "Edwards, Esmond" });
			_agentRepository.Create(new Agent { Name = "Ethan Coen", NameSort = "Coen, Ethan" });
			_agentRepository.Create(new Agent { Name = "Ethiopians", NameSort = "Ethiopians" });
			_agentRepository.Create(new Agent { Name = "Etoile de Manaco", NameSort = "Etoile de Manaco" });
			_agentRepository.Create(new Agent { Name = "Eugene Chadbourne", NameSort = "Chadbourne, Eugene" });
			_agentRepository.Create(new Agent { Name = "Eugene Foster", NameSort = "Foster, Eugene" });
			_agentRepository.Create(new Agent { Name = "Eugene Record", NameSort = "Record, Eugene" });
			_agentRepository.Create(new Agent { Name = "Ewan MacColl", NameSort = "MacColl, Ewan" });
			_agentRepository.Create(new Agent { Name = "F. Byron Clark", NameSort = "Clark, F. Byron" });
			_agentRepository.Create(new Agent { Name = "F. Kwakye", NameSort = "Kwakye, F." });
			_agentRepository.Create(new Agent { Name = "Faces", NameSort = "Faces" });
			_agentRepository.Create(new Agent { Name = "Fairport Convention", NameSort = "Fairport Convention" });
			_agentRepository.Create(new Agent { Name = "Fats Kaplin", NameSort = "Kaplin, Fats" });
			_agentRepository.Create(new Agent { Name = "Fatta & Karl Toppin", NameSort = "Toppin, Fatta & Karl" });
			_agentRepository.Create(new Agent { Name = "Faust", NameSort = "Faust" });
			_agentRepository.Create(new Agent { Name = "Feist", NameSort = "Feist" });
			_agentRepository.Create(new Agent { Name = "Fela Kuti & Africa 70", NameSort = "Kuti & Africa 70, Fela", FileUnder = "Fela Kuti" });
			_agentRepository.Create(new Agent { Name = "Fela Kuti & Egypt 80", NameSort = "Kuti & Egypt 80, Fela", FileUnder = "Fela Kuti" });
			_agentRepository.Create(new Agent { Name = "Fela Kuti", NameSort = "Kuti, Fela" });
			_agentRepository.Create(new Agent { Name = "Felix Pappalardi", NameSort = "Pappalardi, Felix" });
			_agentRepository.Create(new Agent { Name = "Felton Jarvis", NameSort = "Jarvis, Felton" });
			_agentRepository.Create(new Agent { Name = "Firehose", NameSort = "Firehose" });
			_agentRepository.Create(new Agent { Name = "Flatt & Scruggs", NameSort = "Flatt & Scruggs" });
			_agentRepository.Create(new Agent { Name = "Flora Purim", NameSort = "Purim, Flora" });
			_agentRepository.Create(new Agent { Name = "France Gall", NameSort = "Gall, France" });
			_agentRepository.Create(new Agent { Name = "Francis Bebey", NameSort = "Bebey, Francis" });
			_agentRepository.Create(new Agent { Name = "Francis Lai", NameSort = "Lai, Francis" });
			_agentRepository.Create(new Agent { Name = "Francis Wolff", NameSort = "Wolff, Francis" });
			_agentRepository.Create(new Agent { Name = "Franco et le T.P.O.K. Jazz", NameSort = "Franco et le T.P.O.K. Jazz", FileUnder = "Franco" });
			_agentRepository.Create(new Agent { Name = "Frank Jones", NameSort = "Jones, Frank" });
			_agentRepository.Create(new Agent { Name = "Frank Laico", NameSort = "Laico, Frank" });
			_agentRepository.Create(new Agent { Name = "Frank Zappa & Mothers of Invention", NameSort = "Zappa & Mothers of Invention, Frank" });
			_agentRepository.Create(new Agent { Name = "Frank Zappa", NameSort = "Zappa, Frank" });
			_agentRepository.Create(new Agent { Name = "Frankie Goes To Hollywood", NameSort = "Frankie Goes To Hollywood" });
			_agentRepository.Create(new Agent { Name = "Fred Brockman", NameSort = "Brockman, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Cataro", NameSort = "Cataro, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Catero", NameSort = "Cataro, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Frith", NameSort = "Frith, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Maher", NameSort = "Maher, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Weinberg", NameSort = "Weinberg, Fred" });
			_agentRepository.Create(new Agent { Name = "Fred Wesley & The J.B.'s", NameSort = "Wesley & The J.B.'s, Fred", FileUnder = "Fred Wesley" });
			_agentRepository.Create(new Agent { Name = "Fred Wesley & The New J.B.'s", NameSort = "Wesley & The J.B.'s, Fred", FileUnder = "Fred Wesley" });
			_agentRepository.Create(new Agent { Name = "Fred Wesley and the Horny Horns", NameSort = "Wesley and the Horny Horns, Fred", FileUnder = "Fred Wesley" });
			_agentRepository.Create(new Agent { Name = "Fred Wesley", NameSort = "Wesley, Fred" });
			_agentRepository.Create(new Agent { Name = "Freddie Hubbard", NameSort = "Hubbard, Freddie" });
			_agentRepository.Create(new Agent { Name = "Freddie King", NameSort = "King, Freddie" });
			_agentRepository.Create(new Agent { Name = "Freddie McGreggor", NameSort = "McGreggor, Freddie" });
			_agentRepository.Create(new Agent { Name = "Frederic Ramsey Jr.", NameSort = "Ramsey Jr., Frederic" });
			_agentRepository.Create(new Agent { Name = "Freedom Sounds", NameSort = "Freedom Sounds" });
			_agentRepository.Create(new Agent { Name = "Fu Manchu", NameSort = "Fu Manchu" });
			_agentRepository.Create(new Agent { Name = "Fugazi", NameSort = "Fugazi" });
			_agentRepository.Create(new Agent { Name = "Funkadelic", NameSort = "Funkadelic" });
			_agentRepository.Create(new Agent { Name = "Fuzzy Owen", NameSort = "Owen, Fuzzy" });
			_agentRepository.Create(new Agent { Name = "G.G. All Stars", NameSort = "G.G. All Stars" });
			_agentRepository.Create(new Agent { Name = "Gal Costa", NameSort = "Costa, Gal" });
			_agentRepository.Create(new Agent { Name = "Gang of Four", NameSort = "Gang of Four" });
			_agentRepository.Create(new Agent { Name = "Gary Kellgren", NameSort = "Kellgren, Gary" });
			_agentRepository.Create(new Agent { Name = "Gary Ladinsky", NameSort = "Ladinsky, Gary" });
			_agentRepository.Create(new Agent { Name = "Gary Puckett & The Union Gap", NameSort = "Puckett & The Union Gap, Gary", FileUnder = "Gary Puckett" });
			_agentRepository.Create(new Agent { Name = "Gary Smith", NameSort = "Smith, Gary" });
			_agentRepository.Create(new Agent { Name = "Gary Usher", NameSort = "Usher, Gary" });
			_agentRepository.Create(new Agent { Name = "Gas Huffer", NameSort = "Gas Huffer" });
			_agentRepository.Create(new Agent { Name = "Gato Barbieri", NameSort = "Barbieri, Gato" });
			_agentRepository.Create(new Agent { Name = "GBH", NameSort = "GBH" });
			_agentRepository.Create(new Agent { Name = "Gene Chandler", NameSort = "Chandler, Gene" });
			_agentRepository.Create(new Agent { Name = "Gene Holder", NameSort = "Holder, Gene" });
			_agentRepository.Create(new Agent { Name = "Gene Norman", NameSort = "Norman, Gene" });
			_agentRepository.Create(new Agent { Name = "Gene Paul", NameSort = "Paul, Gene" });
			_agentRepository.Create(new Agent { Name = "Gene Pitney", NameSort = "Pitney, Gene" });
			_agentRepository.Create(new Agent { Name = "General Public", NameSort = "General Public" });
			_agentRepository.Create(new Agent { Name = "Generation X", NameSort = "Generation X" });
			_agentRepository.Create(new Agent { Name = "Genesis", NameSort = "Genesis" });
			_agentRepository.Create(new Agent { Name = "Gentle Giant", NameSort = "Gentle Giant" });
			_agentRepository.Create(new Agent { Name = "Geoff Emerick", NameSort = "Emerick, Geoff" });
			_agentRepository.Create(new Agent { Name = "Geoff Workman", NameSort = "Workman, Geoff" });
			_agentRepository.Create(new Agent { Name = "Geoffrey Chung", NameSort = "Chung, Geoffrey" });
			_agentRepository.Create(new Agent { Name = "George Abdo", NameSort = "Abdo, George" });
			_agentRepository.Create(new Agent { Name = "George Bennett", NameSort = "Bennett, George" });
			_agentRepository.Create(new Agent { Name = "George Chkiantz", NameSort = "Chkiantz, George" });
			_agentRepository.Create(new Agent { Name = "George Clinton", NameSort = "Clinton, George" });
			_agentRepository.Create(new Agent { Name = "George Duke", NameSort = "Duke, George" });
			_agentRepository.Create(new Agent { Name = "George Harrison", NameSort = "Harrison, George" });
			_agentRepository.Create(new Agent { Name = "George Jones", NameSort = "Jones, George" });
			_agentRepository.Create(new Agent { Name = "George Klabin", NameSort = "Klabin, George" });
			_agentRepository.Create(new Agent { Name = "George Martin", NameSort = "Martin, George" });
			_agentRepository.Create(new Agent { Name = "George Massenburg", NameSort = "Massenburg, George" });
			_agentRepository.Create(new Agent { Name = "George Philpott", NameSort = "Philpott, George" });
			_agentRepository.Create(new Agent { Name = "Gerald Oshita", NameSort = "Oshita, Gerald" });
			_agentRepository.Create(new Agent { Name = "Ghost", NameSort = "Ghost" });
			_agentRepository.Create(new Agent { Name = "Gil Evans", NameSort = "Evans, Gil" });
			_agentRepository.Create(new Agent { Name = "Gil Fuller", NameSort = "Fuller, Gil" });
			_agentRepository.Create(new Agent { Name = "Ginger Baker", NameSort = "Baker, Ginger" });
			_agentRepository.Create(new Agent { Name = "Girls Names", NameSort = "Girls Names" });
			_agentRepository.Create(new Agent { Name = "Gladiators", NameSort = "Gladiators" });
			_agentRepository.Create(new Agent { Name = "Glahé Musette Orchestra", NameSort = "Glahé Musette Orchestra" });
			_agentRepository.Create(new Agent { Name = "Glen Brown", NameSort = "Brown, Glen" });
			_agentRepository.Create(new Agent { Name = "Glen Campbell", NameSort = "Campbell, Glen" });
			_agentRepository.Create(new Agent { Name = "Glenmore Brown", NameSort = "Brown, Glenmore", FileUnder = "Glen Brown" });
			_agentRepository.Create(new Agent { Name = "Glenn Gould", NameSort = "Gould, Glenn" });
			_agentRepository.Create(new Agent { Name = "Glenn Himmaugh", NameSort = "Himmaugh, Glenn" });
			_agentRepository.Create(new Agent { Name = "Glyn Johns", NameSort = "Johns, Glyn" });
			_agentRepository.Create(new Agent { Name = "Godwin Logie", NameSort = "Logie, Godwin" });
			_agentRepository.Create(new Agent { Name = "Gordon Calcote", NameSort = "Calcote, Gordon" });
			_agentRepository.Create(new Agent { Name = "Graham Nash", NameSort = "Nash, Graham" });
			_agentRepository.Create(new Agent { Name = "Gram Parsons", NameSort = "Parsons, Gram" });
			_agentRepository.Create(new Agent { Name = "Grant Hart", NameSort = "Hart, Grant" });
			_agentRepository.Create(new Agent { Name = "Grateful Dead", NameSort = "Grateful Dead" });
			_agentRepository.Create(new Agent { Name = "Greg Edward", NameSort = "Edward, Greg" });
			_agentRepository.Create(new Agent { Name = "Greg Saunier", NameSort = "Saunier, Greg" });
			_agentRepository.Create(new Agent { Name = "Gregory Isaacs", NameSort = "Isaacs, Gregory" });
			_agentRepository.Create(new Agent { Name = "Guadalcanal Diary", NameSort = "Guadalcanal Diary" });
			_agentRepository.Create(new Agent { Name = "Guided By Voices", NameSort = "Guided By Voices" });
			_agentRepository.Create(new Agent { Name = "Guilherme Araujo", NameSort = "Araujo, Guilherme" });
			_agentRepository.Create(new Agent { Name = "Guy Lombardo", NameSort = "Lombardo, Guy" });
			_agentRepository.Create(new Agent { Name = "Guy Massey", NameSort = "Massey, Guy" });
			_agentRepository.Create(new Agent { Name = "Guy Stevens", NameSort = "Stevens, Guy" });
			_agentRepository.Create(new Agent { Name = "H.W. \"Pappy\" Daily", NameSort = "Daily, H.W. \"Pappy\"" });
			_agentRepository.Create(new Agent { Name = "Hadrami Ould Medeh", NameSort = "Medeh, Hadrami Ould" });
			_agentRepository.Create(new Agent { Name = "Hammond Scott", NameSort = "Scott, Hammond" });
			_agentRepository.Create(new Agent { Name = "Hank Ballard & The Midnighters", NameSort = "Ballard & The Midnighters, Hank" });
			_agentRepository.Create(new Agent { Name = "Harmonia", NameSort = "Harmonia" });
			_agentRepository.Create(new Agent { Name = "Harold Budd", NameSort = "Budd, Harold" });
			_agentRepository.Create(new Agent { Name = "Harry Belafonte", NameSort = "Belafonte, Harry" });
			_agentRepository.Create(new Agent { Name = "Harry De Winter", NameSort = "De Winter, Harry" });
			_agentRepository.Create(new Agent { Name = "Harry J All-Stars", NameSort = "Harry J All-Stars" });
			_agentRepository.Create(new Agent { Name = "Harry Johnson", NameSort = "Johnson, Harry" });
			_agentRepository.Create(new Agent { Name = "Harry Nilsson", NameSort = "Nilsson, Harry" });
			_agentRepository.Create(new Agent { Name = "Harvey Brooks", NameSort = "Brooks, Harvey" });
			_agentRepository.Create(new Agent { Name = "Heavenly", NameSort = "Heavenly" });
			_agentRepository.Create(new Agent { Name = "Helium", NameSort = "Helium" });
			_agentRepository.Create(new Agent { Name = "Helvetia", NameSort = "Helvetia" });
			_agentRepository.Create(new Agent { Name = "Henry \"Junjo\" Lawes", NameSort = "Lawes, Henry \"Junjo\"" });
			_agentRepository.Create(new Agent { Name = "Henry Kaiser", NameSort = "Kaiser, Henry" });
			_agentRepository.Create(new Agent { Name = "Henry Mancini", NameSort = "Mancini, Henry" });
			_agentRepository.Create(new Agent { Name = "Henry McNabb", NameSort = "McNabb, Henry" });
			_agentRepository.Create(new Agent { Name = "Herb Alpert's Tijuana Brass", NameSort = "Alpert's Tijuana Brass, Herb", FileUnder = "Herb Alpert" });
			_agentRepository.Create(new Agent { Name = "Herbie Hancock", NameSort = "Hancock, Herbie" });
			_agentRepository.Create(new Agent { Name = "Herbie Mann", NameSort = "Mann, Herbie" });
			_agentRepository.Create(new Agent { Name = "Hermeto Pascoal", NameSort = "Pascoal, Hermeto" });
			_agentRepository.Create(new Agent { Name = "Highwoods Stringband", NameSort = "Highwoods Stringband" });
			_agentRepository.Create(new Agent { Name = "Horace Andy", NameSort = "Andy, Horace" });
			_agentRepository.Create(new Agent { Name = "Horst Weber", NameSort = "Weber, Horst" });
			_agentRepository.Create(new Agent { Name = "Hot Tuna", NameSort = "Hot Tuna" });
			_agentRepository.Create(new Agent { Name = "Hound Dog Taylor", NameSort = "Taylor, Hound Dog" });
			_agentRepository.Create(new Agent { Name = "Howie Albert", NameSort = "Albert, Howie" });
			_agentRepository.Create(new Agent { Name = "Hugh Davies", NameSort = "Davies, Hugh" });
			_agentRepository.Create(new Agent { Name = "Hugh Masekela", NameSort = "Masekela, Hugh" });
			_agentRepository.Create(new Agent { Name = "Hugo Largo", NameSort = "Hugo Largo" });
			_agentRepository.Create(new Agent { Name = "Humble Pie", NameSort = "Humble Pie" });
			_agentRepository.Create(new Agent { Name = "Hume", NameSort = "Hume" });
			_agentRepository.Create(new Agent { Name = "Hüsker Dü", NameSort = "Hüsker Dü" });
			_agentRepository.Create(new Agent { Name = "I Roy", NameSort = "I Roy" });
			_agentRepository.Create(new Agent { Name = "Ian McLagan", NameSort = "McLagan, Ian" });
			_agentRepository.Create(new Agent { Name = "Iggy Pop", NameSort = "Iggy Pop" });
			_agentRepository.Create(new Agent { Name = "Ikebe Shakedown", NameSort = "Ikebe Shakedown" });
			_agentRepository.Create(new Agent { Name = "Ikenga Super Stars of Africa", NameSort = "Ikenga Super Stars of Africa" });
			_agentRepository.Create(new Agent { Name = "Impact All Stars", NameSort = "Impact All Stars" });
			_agentRepository.Create(new Agent { Name = "Insurgence DC", NameSort = "Insurgence DC" });
			_agentRepository.Create(new Agent { Name = "Iron Butterfly", NameSort = "Iron Butterfly" });
			_agentRepository.Create(new Agent { Name = "I-Roy", NameSort = "I-Roy" });
			_agentRepository.Create(new Agent { Name = "IRP-3", NameSort = "IRP-3" });
			_agentRepository.Create(new Agent { Name = "Isaac Hayes", NameSort = "Hayes, Isaac" });
			_agentRepository.Create(new Agent { Name = "Isotope", NameSort = "Isotope" });
			_agentRepository.Create(new Agent { Name = "It's A Beautiful Day", NameSort = "It's A Beautiful Day" });
			_agentRepository.Create(new Agent { Name = "J. Coxon", NameSort = "Coxon, J." });
			_agentRepository.Create(new Agent { Name = "J. Enwright", NameSort = "Enwright, J." });
			_agentRepository.Create(new Agent { Name = "J. Mascis", NameSort = "Mascis, J." });
			_agentRepository.Create(new Agent { Name = "J. Spaceman", NameSort = "J. Spaceman" });
			_agentRepository.Create(new Agent { Name = "J. Yuenger", NameSort = "Yuenger, J." });
			_agentRepository.Create(new Agent { Name = "J.J. Johnson", NameSort = "Johnson, J.J." });
			_agentRepository.Create(new Agent { Name = "Jack Nuber", NameSort = "Nuber, Jack" });
			_agentRepository.Create(new Agent { Name = "Jack Teagarden", NameSort = "Teagarden, Jack" });
			_agentRepository.Create(new Agent { Name = "Jack White III", NameSort = "White III, Jack" });
			_agentRepository.Create(new Agent { Name = "Jackie Mittoo & Randy's All Stars", NameSort = "Mittoo & Randy's All Stars, Jackie", FileUnder = "Jackie Mittoo" });
			_agentRepository.Create(new Agent { Name = "Jackie Mittoo & The Soul Vendors", NameSort = "Mittoo & The Soul Vendors, Jackie", FileUnder = "Jackie Mittoo" });
			_agentRepository.Create(new Agent { Name = "Jackie Mittoo", NameSort = "Mittoo, Jackie" });
			_agentRepository.Create(new Agent { Name = "Jackie Opel", NameSort = "Opel, Jackie" });
			_agentRepository.Create(new Agent { Name = "Jacquire King", NameSort = "King, Jacquire" });
			_agentRepository.Create(new Agent { Name = "Jah Lion", NameSort = "Jah Lion" });
			_agentRepository.Create(new Agent { Name = "Jah Screw", NameSort = "Jah Screw" });
			_agentRepository.Create(new Agent { Name = "Jah Thomas", NameSort = "Jah Thomas" });
			_agentRepository.Create(new Agent { Name = "Jah Thunder", NameSort = "Jah Thunder" });
			_agentRepository.Create(new Agent { Name = "James Booker", NameSort = "Booker, James" });
			_agentRepository.Create(new Agent { Name = "James Brown", NameSort = "Brown, James" });
			_agentRepository.Create(new Agent { Name = "James Gang", NameSort = "James Gang" });
			_agentRepository.Create(new Agent { Name = "James McNew", NameSort = "McNew, James" });
			_agentRepository.Create(new Agent { Name = "Jami Ayinde", NameSort = "Ayinde, Jami" });
			_agentRepository.Create(new Agent { Name = "Jan Erik Kongshaug", NameSort = "Kongshaug, Jan Erik" });
			_agentRepository.Create(new Agent { Name = "Jane's Addiction", NameSort = "Jane's Addiction" });
			_agentRepository.Create(new Agent { Name = "Janie Hendrix", NameSort = "Hendrix, Janie" });
			_agentRepository.Create(new Agent { Name = "Janis Joplin", NameSort = "Joplin, Janis" });
			_agentRepository.Create(new Agent { Name = "Jason Albertini", NameSort = "Albertini, Jason" });
			_agentRepository.Create(new Agent { Name = "Jason Wormer", NameSort = "Wormer, Jason" });
			_agentRepository.Create(new Agent { Name = "Javelin", NameSort = "Javelin" });
			_agentRepository.Create(new Agent { Name = "Jay Gallagher", NameSort = "Gallagher, Jay" });
			_agentRepository.Create(new Agent { Name = "Jay Messina", NameSort = "Messina, Jay" });
			_agentRepository.Create(new Agent { Name = "Jay Reatard", NameSort = "Reatard, Jay" });
			_agentRepository.Create(new Agent { Name = "Jean Michel Jarre", NameSort = "Jarre, Jean Michel" });
			_agentRepository.Create(new Agent { Name = "Jean-Claude Vannier", NameSort = "Vannier, Jean-Claude" });
			_agentRepository.Create(new Agent { Name = "Jeff Zeigler", NameSort = "Zeigler, Jeff" });
			_agentRepository.Create(new Agent { Name = "Jefferson Airplane", NameSort = "Jefferson Airplane" });
			_agentRepository.Create(new Agent { Name = "Jennifer Lara", NameSort = "Lara, Jennifer" });
			_agentRepository.Create(new Agent { Name = "Jeremy Zatkin", NameSort = "Zatkin, Jeremy" });
			_agentRepository.Create(new Agent { Name = "Jerry Boys", NameSort = "Jerry Boys" });
			_agentRepository.Create(new Agent { Name = "Jerry Busher", NameSort = "Busher, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jerry Garcia", NameSort = "Garcia, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jerry Green", NameSort = "Green, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jerry Lee Lewis", NameSort = "Lewis, Jerry Lee" });
			_agentRepository.Create(new Agent { Name = "Jerry Masters", NameSort = "Masters, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jerry Wexler", NameSort = "Wexler, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jerry Zatkin", NameSort = "Zatkin, Jerry" });
			_agentRepository.Create(new Agent { Name = "Jethro Tull", NameSort = "Jethro Tull" });
			_agentRepository.Create(new Agent { Name = "Jim Callon", NameSort = "Callon, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Dickinson", NameSort = "Dickinson, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Dickson", NameSort = "Dickinson, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Hawkins", NameSort = "Hawkins, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Kissling", NameSort = "Kissling, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Nabors", NameSort = "Nabors, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Nipar", NameSort = "Nipar, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Stern", NameSort = "Stern, Jim" });
			_agentRepository.Create(new Agent { Name = "Jim Vitti", NameSort = "Vitti, Jim" });
			_agentRepository.Create(new Agent { Name = "Jimi Hendrix", NameSort = "Hendrix, Jimi" });
			_agentRepository.Create(new Agent { Name = "Jimmy Buffett", NameSort = "Buffett, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Cliff", NameSort = "Cliff, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Douglass", NameSort = "Douglass, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Iovine", NameSort = "Iovine, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy McGriff", NameSort = "McGriff, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Miller", NameSort = "Miller, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Page", NameSort = "Page, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Reed", NameSort = "Reed, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Jimmy Smith", NameSort = "Smith, Jimmy" });
			_agentRepository.Create(new Agent { Name = "Joe Boyd", NameSort = "Boyd, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Farrell", NameSort = "Farrell, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Gibbs and the Professionals", NameSort = "Gibbs and the Professionals, Joe", FileUnder = "Joe Gibbs" });
			_agentRepository.Create(new Agent { Name = "Joe Gibbs", NameSort = "Gibbs, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Hardy", NameSort = "Hardy, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Jackson", NameSort = "Jackson, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Restivo", NameSort = "Restivo, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Turner", NameSort = "Turner, Joe" });
			_agentRepository.Create(new Agent { Name = "Joe Williams", NameSort = "Williams, Joe" });
			_agentRepository.Create(new Agent { Name = "Joel Coen", NameSort = "Coen, Joel" });
			_agentRepository.Create(new Agent { Name = "Joel Dorn", NameSort = "Dorn, Joel" });
			_agentRepository.Create(new Agent { Name = "Joey Burns", NameSort = "Burns, Joey" });
			_agentRepository.Create(new Agent { Name = "John Agnello", NameSort = "Agnello, John" });
			_agentRepository.Create(new Agent { Name = "John Anthony", NameSort = "Anthony, John" });
			_agentRepository.Create(new Agent { Name = "John Buddy Williams", NameSort = "Williams, John Buddy" });
			_agentRepository.Create(new Agent { Name = "John C. Fogerty", NameSort = "Fogerty, John C.", FileUnder = "John Fogerty" });
			_agentRepository.Create(new Agent { Name = "John Cale", NameSort = "Cale, John" });
			_agentRepository.Create(new Agent { Name = "John Cevetello", NameSort = "Cevetello, John" });
			_agentRepository.Create(new Agent { Name = "John Coltrane", NameSort = "Coltrane, John" });
			_agentRepository.Create(new Agent { Name = "John Convertino", NameSort = "Convertino, John" });
			_agentRepository.Create(new Agent { Name = "John Elijah Wright", NameSort = "Wright, John Elijah" });
			_agentRepository.Create(new Agent { Name = "John Florez", NameSort = "Florez, John" });
			_agentRepository.Create(new Agent { Name = "John Fogerty", NameSort = "Fogerty, John" });
			_agentRepository.Create(new Agent { Name = "John Fry", NameSort = "Fry, John" });
			_agentRepository.Create(new Agent { Name = "John Hammond", NameSort = "Hammond, John" });
			_agentRepository.Create(new Agent { Name = "John Hampton", NameSort = "Hampton, John" });
			_agentRepository.Create(new Agent { Name = "John Hewlett", NameSort = "Hewlett, John" });
			_agentRepository.Create(new Agent { Name = "John Holt", NameSort = "Holt, John" });
			_agentRepository.Create(new Agent { Name = "John Kahn", NameSort = "Kahn, John" });
			_agentRepository.Create(new Agent { Name = "John Leckie", NameSort = "Leckie, John" });
			_agentRepository.Create(new Agent { Name = "John Lee Hooker", NameSort = "Hooker, John Lee" });
			_agentRepository.Create(new Agent { Name = "John Lennon", NameSort = "Lennon, John" });
			_agentRepository.Create(new Agent { Name = "John Lurie", NameSort = "Lurie, John" });
			_agentRepository.Create(new Agent { Name = "John McDermott", NameSort = "McDermott, John" });
			_agentRepository.Create(new Agent { Name = "John McEntire", NameSort = "McEntire, John" });
			_agentRepository.Create(new Agent { Name = "John McLaughlin", NameSort = "McLaughlin, John" });
			_agentRepository.Create(new Agent { Name = "John Palladino", NameSort = "Palladino, John" });
			_agentRepository.Create(new Agent { Name = "John Patton", NameSort = "Patton, John" });
			_agentRepository.Create(new Agent { Name = "John Prine", NameSort = "Prine, John" });
			_agentRepository.Create(new Agent { Name = "John Siket", NameSort = "Siket, John" });
			_agentRepository.Create(new Agent { Name = "John Simon", NameSort = "Simon, John" });
			_agentRepository.Create(new Agent { Name = "John Svek", NameSort = "Svek, John" });
			_agentRepository.Create(new Agent { Name = "John Wood", NameSort = "Wood, John" });
			_agentRepository.Create(new Agent { Name = "John Zorn", NameSort = "Zorn, John" });
			_agentRepository.Create(new Agent { Name = "Johnny Adams", NameSort = "Adams, Johnny" });
			_agentRepository.Create(new Agent { Name = "Johnny Cash", NameSort = "Cash, Johnny" });
			_agentRepository.Create(new Agent { Name = "Johnny Hartman", NameSort = "Hartman, Johnny" });
			_agentRepository.Create(new Agent { Name = "Johnny Irion", NameSort = "Irion, Johnny" });
			_agentRepository.Create(new Agent { Name = "Johnny Marr", NameSort = "Marr, Johnny" });
			_agentRepository.Create(new Agent { Name = "Johnny Otis", NameSort = "Otis, Johnny" });
			_agentRepository.Create(new Agent { Name = "Joni Mitchell", NameSort = "Mitchell, Joni" });
			_agentRepository.Create(new Agent { Name = "Jorma Kaukonen", NameSort = "Kaukonen, Jorma" });
			_agentRepository.Create(new Agent { Name = "Joseph Hoo Kim", NameSort = "Kim, Joseph Hoo" });
			_agentRepository.Create(new Agent { Name = "Joseph Vito", NameSort = "Vito, Joseph" });
			_agentRepository.Create(new Agent { Name = "Joy Division", NameSort = "Joy Division" });
			_agentRepository.Create(new Agent { Name = "Joy Zipper", NameSort = "Joy Zipper" });
			_agentRepository.Create(new Agent { Name = "Jr. Walker & The All Stars", NameSort = "Walker & The All Stars, Jr. " });
			_agentRepository.Create(new Agent { Name = "Jr. Walker & The All Stars", NameSort = "Walker & The All Stars, Jr." });
			_agentRepository.Create(new Agent { Name = "Judas Priest", NameSort = "Judas Priest" });
			_agentRepository.Create(new Agent { Name = "June Of 44", NameSort = "June Of 44" });
			_agentRepository.Create(new Agent { Name = "Junior Murvin", NameSort = "Murvin, Junior" });
			_agentRepository.Create(new Agent { Name = "Junior Wells", NameSort = "Wells, Junior" });
			_agentRepository.Create(new Agent { Name = "Justin Pizzoferrato", NameSort = "Pizzoferrato, Justin" });
			_agentRepository.Create(new Agent { Name = "K. Archer", NameSort = "Archer, K." });
			_agentRepository.Create(new Agent { Name = "K. Kwakye", NameSort = "Kwakye, K." });
			_agentRepository.Create(new Agent { Name = "Karl Blau", NameSort = "Blau, Karl" });
			_agentRepository.Create(new Agent { Name = "Karl Pitterson", NameSort = "Pitterson, Karl" });
			_agentRepository.Create(new Agent { Name = "Kathy Dennis", NameSort = "Dennis, Kathy" });
			_agentRepository.Create(new Agent { Name = "Keiland Holleman", NameSort = "Holleman, Keiland" });
			_agentRepository.Create(new Agent { Name = "Keith Hudson", NameSort = "Hudson, Keith" });
			_agentRepository.Create(new Agent { Name = "Keith Richards", NameSort = "Richards, Keith" });
			_agentRepository.Create(new Agent { Name = "Keith", NameSort = "Keith" });
			_agentRepository.Create(new Agent { Name = "Kemastri", NameSort = "Kemastri" });
			_agentRepository.Create(new Agent { Name = "Ken Boothe", NameSort = "Boothe, Ken" });
			_agentRepository.Create(new Agent { Name = "Ken Mansfield", NameSort = "Mansfield, Ken" });
			_agentRepository.Create(new Agent { Name = "Ken Nelson", NameSort = "Nelson, Ken" });
			_agentRepository.Create(new Agent { Name = "Kendall Stubbs", NameSort = "Stubbs, Kendall" });
			_agentRepository.Create(new Agent { Name = "Kenneth Hamann", NameSort = "Hamann, Kenneth" });
			_agentRepository.Create(new Agent { Name = "Kent 3", NameSort = "Kent 3" });
			_agentRepository.Create(new Agent { Name = "Khamis El Fino Ali", NameSort = "Khamis El Fino Ali" });
			_agentRepository.Create(new Agent { Name = "Kiddus I", NameSort = "Kiddus I" });
			_agentRepository.Create(new Agent { Name = "Killing Joke", NameSort = "Killing Joke" });
			_agentRepository.Create(new Agent { Name = "Kim Loy Wong", NameSort = "Wong, Kim Loy" });
			_agentRepository.Create(new Agent { Name = "King Stitt", NameSort = "King Stitt" });
			_agentRepository.Create(new Agent { Name = "King Tubby", NameSort = "King Tubby" });
			_agentRepository.Create(new Agent { Name = "King Tuff", NameSort = "King Tuff" });
			_agentRepository.Create(new Agent { Name = "Kingsmen", NameSort = "Kingsmen" });
			_agentRepository.Create(new Agent { Name = "Klaus Dinger", NameSort = "Dinger, Klaus" });
			_agentRepository.Create(new Agent { Name = "Kool Moe Dee", NameSort = "Kool Moe Dee" });
			_agentRepository.Create(new Agent { Name = "Kraan", NameSort = "Kraan" });
			_agentRepository.Create(new Agent { Name = "Kraftwerk", NameSort = "Kraftwerk" });
			_agentRepository.Create(new Agent { Name = "Kramer", NameSort = "Kramer" });
			_agentRepository.Create(new Agent { Name = "Kris Kristofferson", NameSort = "Kristofferson, Kris" });
			_agentRepository.Create(new Agent { Name = "Kurt Rapp", NameSort = "Rapp, Kurt" });
			_agentRepository.Create(new Agent { Name = "Kurt Schlegel", NameSort = "Kurt Schlegel" });
			_agentRepository.Create(new Agent { Name = "L. James", NameSort = "James, L." });
			_agentRepository.Create(new Agent { Name = "L. Lindo (Jack Ruby)", NameSort = "Lindo (Jack Ruby), L." });
			_agentRepository.Create(new Agent { Name = "L. McDonald", NameSort = "McDonald, L." });
			_agentRepository.Create(new Agent { Name = "Lady Gaga", NameSort = "Lady Gaga" });
			_agentRepository.Create(new Agent { Name = "Ladysmith Black Mambazo", NameSort = "Ladysmith Black Mambazo" });
			_agentRepository.Create(new Agent { Name = "Lance Koehler", NameSort = "Koehler, Lance" });
			_agentRepository.Create(new Agent { Name = "Lanky Linstrot", NameSort = "Linstrot, Lanky" });
			_agentRepository.Create(new Agent { Name = "Larry Alexander", NameSort = "Alexander, Larry" });
			_agentRepository.Create(new Agent { Name = "Larry Hamby", NameSort = "Hamby, Larry" });
			_agentRepository.Create(new Agent { Name = "Larry Smith", NameSort = "Smith, Larry" });
			_agentRepository.Create(new Agent { Name = "Larry Young", NameSort = "Young, Larry" });
			_agentRepository.Create(new Agent { Name = "Last Exit", NameSort = "Last Exit" });
			_agentRepository.Create(new Agent { Name = "Le Grand Kalle & The African Team", NameSort = "Grand Kalle & The African Team, Le" });
			_agentRepository.Create(new Agent { Name = "Leadbelly", NameSort = "Lead Belly" });
			_agentRepository.Create(new Agent { Name = "Leanord Cohen", NameSort = "Cohen, Leanord" });
			_agentRepository.Create(new Agent { Name = "Led Zeppelin", NameSort = "Led Zeppelin" });
			_agentRepository.Create(new Agent { Name = "Lee Dorsey", NameSort = "Dorsey, Lee" });
			_agentRepository.Create(new Agent { Name = "Lee Hazen", NameSort = "Hazen, Lee" });
			_agentRepository.Create(new Agent { Name = "Lee Morgan", NameSort = "Morgan, Lee" });
			_agentRepository.Create(new Agent { Name = "Lee Perry and the Upsetters", NameSort = "Perry and the Upsetters, Lee", FileUnder = "Lee Perry" });
			_agentRepository.Create(new Agent { Name = "Lee Perry The Mighty Upsetter", NameSort = "Perry The Mighty Upsetter, Lee", FileUnder = "Lee Perry" });
			_agentRepository.Create(new Agent { Name = "Lee Perry", NameSort = "Perry, Lee" });
			_agentRepository.Create(new Agent { Name = "Lee Van Cliff", NameSort = "Cliff, Lee Van" });
			_agentRepository.Create(new Agent { Name = "Leif Mases", NameSort = "Mases, Leif" });
			_agentRepository.Create(new Agent { Name = "Lemonheads", NameSort = "Lemonheads" });
			_agentRepository.Create(new Agent { Name = "Lennie Hibbert", NameSort = "Hibbert, Lennie" });
			_agentRepository.Create(new Agent { Name = "Leo Kottke", NameSort = "Kottke, Leo" });
			_agentRepository.Create(new Agent { Name = "Leo Smith", NameSort = "Smith & New Dalta Ahkri, Leo", FileUnder = "Leo Smith" });
			_agentRepository.Create(new Agent { Name = "Leo Smith", NameSort = "Smith, Leo" });
			_agentRepository.Create(new Agent { Name = "Leon Russell", NameSort = "Russell, Leon" });
			_agentRepository.Create(new Agent { Name = "Leroy Hutson", NameSort = "Hutson, Leroy" });
			_agentRepository.Create(new Agent { Name = "Leroy Pierson", NameSort = "Pierson, Leroy" });
			_agentRepository.Create(new Agent { Name = "Leroy Smart", NameSort = "Smart, Leroy" });
			_agentRepository.Create(new Agent { Name = "Les Ambassadeurs", NameSort = "Ambassadeurs, Les" });
			_agentRepository.Create(new Agent { Name = "Les Baxter", NameSort = "Baxter, Les" });
			_agentRepository.Create(new Agent { Name = "Les Ladd", NameSort = "Ladd, Les" });
			_agentRepository.Create(new Agent { Name = "Les McCann", NameSort = "McCann, Les" });
			_agentRepository.Create(new Agent { Name = "Les Troubadours Du Roi Baudouin", NameSort = "Troubadours Du Roi Baudouin, Les" });
			_agentRepository.Create(new Agent { Name = "Leslie D. Kippel", NameSort = "Kippel, Leslie D." });
			_agentRepository.Create(new Agent { Name = "Leszek J. Karski", NameSort = "Karski, Leszek J. " });
			_agentRepository.Create(new Agent { Name = "Let's Active", NameSort = "Let's Active" });
			_agentRepository.Create(new Agent { Name = "Levon Helm", NameSort = "Helm, Levon" });
			_agentRepository.Create(new Agent { Name = "Lew Futterman", NameSort = "Futterman, Lew" });
			_agentRepository.Create(new Agent { Name = "Lew Hahn", NameSort = "Hahn, Lew" });
			_agentRepository.Create(new Agent { Name = "Lewis Futterman", NameSort = "Futterman, Lewis" });
			_agentRepository.Create(new Agent { Name = "Lightnin' Hopkins", NameSort = "Lightnin' Hopkins" });
			_agentRepository.Create(new Agent { Name = "Lillian Douma", NameSort = "Douma, Lillian" });
			_agentRepository.Create(new Agent { Name = "Linda Clifford", NameSort = "Linda Clifford" });
			_agentRepository.Create(new Agent { Name = "Linton Kwesi Johnson", NameSort = "Johnson, Linton Kwesi" });
			_agentRepository.Create(new Agent { Name = "Linval Thompson", NameSort = "Thompson, Linval" });
			_agentRepository.Create(new Agent { Name = "Lipps Inc.", NameSort = "Lipps Inc." });
			_agentRepository.Create(new Agent { Name = "Little Beaver", NameSort = "Little Beaver" });
			_agentRepository.Create(new Agent { Name = "Little Feat", NameSort = "Little Feat" });
			_agentRepository.Create(new Agent { Name = "Little John", NameSort = "Little John" });
			_agentRepository.Create(new Agent { Name = "Living Colour", NameSort = "Living Colour" });
			_agentRepository.Create(new Agent { Name = "Lloyd Williams", NameSort = "Williams, Lloyd" });
			_agentRepository.Create(new Agent { Name = "Lockie Edwards", NameSort = "Edwards, Lockie" });
			_agentRepository.Create(new Agent { Name = "Lois", NameSort = "Lois" });
			_agentRepository.Create(new Agent { Name = "London Symphony Orchestra", NameSort = "London Symphony Orchestra" });
			_agentRepository.Create(new Agent { Name = "Lonnie Liston Smith & The Cosmic Echoes", NameSort = "Smith & The Cosmic Echoes, Lonnie Liston", FileUnder = "Lonnie Liston Smith" });
			_agentRepository.Create(new Agent { Name = "Lonnie Mack", NameSort = "Mack, Lonnie" });
			_agentRepository.Create(new Agent { Name = "Lotion", NameSort = "Lotion" });
			_agentRepository.Create(new Agent { Name = "Lou Donaldson", NameSort = "Donaldson, Lou" });
			_agentRepository.Create(new Agent { Name = "Lou Giordano", NameSort = "Giordano, Lou" });
			_agentRepository.Create(new Agent { Name = "Lou Reed", NameSort = "Reed, Lou" });
			_agentRepository.Create(new Agent { Name = "Louis Armstrong & His All-Stars", NameSort = "Armstrong & His All-Stars, Louis", FileUnder = "Louis Armstrong" });
			_agentRepository.Create(new Agent { Name = "Louis Armstrong and His Hot Five", NameSort = "Armstrong and His Hot Five, Louis", FileUnder = "Louis Armstrong" });
			_agentRepository.Create(new Agent { Name = "Louis Armstrong", NameSort = "Armstrong, Louis" });
			_agentRepository.Create(new Agent { Name = "Louis Austin", NameSort = "Austin, Louis" });
			_agentRepository.Create(new Agent { Name = "Loukelo Samba", NameSort = "Samba, Loukelo" });
			_agentRepository.Create(new Agent { Name = "Love As Laughter", NameSort = "Love As Laughter" });
			_agentRepository.Create(new Agent { Name = "Love Tractor", NameSort = "Love Tractor" });
			_agentRepository.Create(new Agent { Name = "Low", NameSort = "Low" });
			_agentRepository.Create(new Agent { Name = "Luciano", NameSort = "Luciano" });
			_agentRepository.Create(new Agent { Name = "Luiz Bonfa", NameSort = "Bonfa, Luiz" });
			_agentRepository.Create(new Agent { Name = "Lukie D", NameSort = "Lukie D" });
			_agentRepository.Create(new Agent { Name = "Luminescent Orchestrii", NameSort = "Luminescent Orchestrii" });
			_agentRepository.Create(new Agent { Name = "Lynford Anderson (Andy Capp)", NameSort = "Anderson (Andy Capp), Lynford" });
			_agentRepository.Create(new Agent { Name = "M.I.A.", NameSort = "M.I.A." });
			_agentRepository.Create(new Agent { Name = "Mac Rebennack", NameSort = "Rebennack, Mac", FileUnder = "Dr. John" });
			_agentRepository.Create(new Agent { Name = "Maceo Parker", NameSort = "Parker, Maceo" });
			_agentRepository.Create(new Agent { Name = "Mack", NameSort = "Mack" });
			_agentRepository.Create(new Agent { Name = "Madness", NameSort = "Madness" });
			_agentRepository.Create(new Agent { Name = "Madonna", NameSort = "Madonna" });
			_agentRepository.Create(new Agent { Name = "Mads Bjerke", NameSort = "Bjerke, Mads" });
			_agentRepository.Create(new Agent { Name = "Mal Waldron", NameSort = "Waldron, Mal" });
			_agentRepository.Create(new Agent { Name = "Malcolm Addey", NameSort = "Addey, Malcolm" });
			_agentRepository.Create(new Agent { Name = "Mamood", NameSort = "Mamood" });
			_agentRepository.Create(new Agent { Name = "Manchester Orchestra", NameSort = "Manchester Orchestra" });
			_agentRepository.Create(new Agent { Name = "Manfred Eicher", NameSort = "Eicher, Manfred" });
			_agentRepository.Create(new Agent { Name = "Manfred Mann", NameSort = "Mann, Manfred" });
			_agentRepository.Create(new Agent { Name = "Mange Lipscomb", NameSort = "Lipscomb, Mange" });
			_agentRepository.Create(new Agent { Name = "Manu Dibango", NameSort = "Dibango, Manu" });
			_agentRepository.Create(new Agent { Name = "Marc Ribot Trio", NameSort = "Ribot Trio, Marc", FileUnder = "Marc Ribot" });
			_agentRepository.Create(new Agent { Name = "Marc Ribot's Ceramic Dog", NameSort = "Ribot's Ceramic Dog, Marc", FileUnder = "Marc Ribot" });
			_agentRepository.Create(new Agent { Name = "Marc Savoy", NameSort = "Savoy, Marc" });
			_agentRepository.Create(new Agent { Name = "Marcello Small", NameSort = "Small, Marcello" });
			_agentRepository.Create(new Agent { Name = "Marcus James", NameSort = "James, Marcus" });
			_agentRepository.Create(new Agent { Name = "Marine Research", NameSort = "Marine Research" });
			_agentRepository.Create(new Agent { Name = "Mark Venezia", NameSort = "Venezia, Mark" });
			_agentRepository.Create(new Agent { Name = "Marlin Greene", NameSort = "Greene, Marlin" });
			_agentRepository.Create(new Agent { Name = "Mars Accelerator", NameSort = "Mars Accelerator" });
			_agentRepository.Create(new Agent { Name = "Marshall E. Sehorn", NameSort = "Sehorn, Marshall E." });
			_agentRepository.Create(new Agent { Name = "Martin Birch", NameSort = "Birch, Martin" });
			_agentRepository.Create(new Agent { Name = "Martin Carthy", NameSort = "Carthy, Martin" });
			_agentRepository.Create(new Agent { Name = "Martin Denny", NameSort = "Denny, Martin" });
			_agentRepository.Create(new Agent { Name = "Martin Hayles", NameSort = "Hayles, Martin" });
			_agentRepository.Create(new Agent { Name = "Marty Robbins", NameSort = "Robbins, Marty" });
			_agentRepository.Create(new Agent { Name = "Marvin Gaye", NameSort = "Gaye, Marvin" });
			_agentRepository.Create(new Agent { Name = "Mary Lou Williams", NameSort = "Williams, Mary Lou" });
			_agentRepository.Create(new Agent { Name = "MaseQua Myers", NameSort = "Myers, MaseQua" });
			_agentRepository.Create(new Agent { Name = "Material", NameSort = "Material" });
			_agentRepository.Create(new Agent { Name = "Matt Beckley", NameSort = "Beckley, Matt" });
			_agentRepository.Create(new Agent { Name = "Matt Horton", NameSort = "Horton, Matt" });
			_agentRepository.Create(new Agent { Name = "Matthew E. White", NameSort = "White, Matthew E." });
			_agentRepository.Create(new Agent { Name = "Matthias Winckelmann", NameSort = "Winckelmann, Matthias" });
			_agentRepository.Create(new Agent { Name = "Mavis Staples", NameSort = "Staples, Mavis" });
			_agentRepository.Create(new Agent { Name = "Max Romeo", NameSort = "Romeo, Max " });
			_agentRepository.Create(new Agent { Name = "Maxie", NameSort = "Maxie" });
			_agentRepository.Create(new Agent { Name = "Maxy Smith", NameSort = "Smith, Maxy", FileUnder = "Max Romeo" });
			_agentRepository.Create(new Agent { Name = "M'Bilia Bel", NameSort = "Bel, M'Bilia" });
			_agentRepository.Create(new Agent { Name = "McCoy Tyner", NameSort = "Tyner, McCoy" });
			_agentRepository.Create(new Agent { Name = "Meat Puppets", NameSort = "Meat Puppets" });
			_agentRepository.Create(new Agent { Name = "Medusa Cyclone", NameSort = "Medusa Cyclone" });
			_agentRepository.Create(new Agent { Name = "Mel & Tim", NameSort = "Mel & Tim" });
			_agentRepository.Create(new Agent { Name = "Melba Montgomery", NameSort = "Montgomery, Melba" });
			_agentRepository.Create(new Agent { Name = "Melvins", NameSort = "Melvins" });
			_agentRepository.Create(new Agent { Name = "Men At Work", NameSort = "Men At Work" });
			_agentRepository.Create(new Agent { Name = "Men Without Hats", NameSort = "Men Without Hats" });
			_agentRepository.Create(new Agent { Name = "Merl Saunders", NameSort = "Saunders, Merl" });
			_agentRepository.Create(new Agent { Name = "Merle Haggard and The Strangers", NameSort = "Haggard and The Strangers, Merle", FileUnder = "Merle Haggard" });
			_agentRepository.Create(new Agent { Name = "Merle Haggard", NameSort = "Haggard, Merle" });
			_agentRepository.Create(new Agent { Name = "Michael Cuscuna", NameSort = "Cuscuna, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Delugg", NameSort = "Delugg, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Doucet with Beausoleil", NameSort = "Doucet with Beausoleil, Michael", FileUnder = "Michael Doucet" });
			_agentRepository.Create(new Agent { Name = "Michael Doucet", NameSort = "Doucet, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Jackson", NameSort = "Jackson, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael John", NameSort = "John, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Prophet", NameSort = "Prophet, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Rother", NameSort = "Rother, Michael" });
			_agentRepository.Create(new Agent { Name = "Michael Thomas Connolly", NameSort = "Connolly, Michael Thomas" });
			_agentRepository.Create(new Agent { Name = "Michael White", NameSort = "White, Michael" });
			_agentRepository.Create(new Agent { Name = "Michelle Shocked", NameSort = "Michelle Shocked" });
			_agentRepository.Create(new Agent { Name = "Michigan & Smiley", NameSort = "Michigan & Smiley" });
			_agentRepository.Create(new Agent { Name = "Mickey Hart", NameSort = "Hart, Mickey" });
			_agentRepository.Create(new Agent { Name = "Mickey Reily", NameSort = "Reily, Mickey" });
			_agentRepository.Create(new Agent { Name = "Mickie Most", NameSort = "Most, Mickie" });
			_agentRepository.Create(new Agent { Name = "Midnight Oil", NameSort = "Midnight Oil" });
			_agentRepository.Create(new Agent { Name = "Mighty Diamonds", NameSort = "Mighty Diamonds" });
			_agentRepository.Create(new Agent { Name = "Mike Auldridge", NameSort = "Auldridge, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Boback", NameSort = "Boback, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Chapman", NameSort = "Chapman, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Cooper", NameSort = "Cooper, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Dunne", NameSort = "Dunne, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Oldfield", NameSort = "Oldfield, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Polizze", NameSort = "Polizze, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Stone", NameSort = "Stone, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Thorne", NameSort = "Thorne, Mike" });
			_agentRepository.Create(new Agent { Name = "Mike Watt", NameSort = "Watt, Mike" });
			_agentRepository.Create(new Agent { Name = "Miles Davis", NameSort = "Davis, Miles" });
			_agentRepository.Create(new Agent { Name = "Miles Davis", NameSort = "Miles Davis" });
			_agentRepository.Create(new Agent { Name = "Millard Fillmore", NameSort = "Fillmore, Millard" });
			_agentRepository.Create(new Agent { Name = "Milt Gabler", NameSort = "Gabler, Milt" });
			_agentRepository.Create(new Agent { Name = "Milton Nascimento", NameSort = "Nascimento, Milton" });
			_agentRepository.Create(new Agent { Name = "Minor Threat", NameSort = "Minor Threat" });
			_agentRepository.Create(new Agent { Name = "Minutemen", NameSort = "Minutemen" });
			_agentRepository.Create(new Agent { Name = "Mississippi \"Fred\" McDowell", NameSort = "McDowell, Mississippi \"Fred\"" });
			_agentRepository.Create(new Agent { Name = "Mississippi John Hurt", NameSort = "Hurt, Mississippi John" });
			_agentRepository.Create(new Agent { Name = "Mitch Easter", NameSort = "Easter, Mitch" });
			_agentRepository.Create(new Agent { Name = "Moby Grape", NameSort = "Moby Grape" });
			_agentRepository.Create(new Agent { Name = "Modest Mouse", NameSort = "Modest Mouse" });
			_agentRepository.Create(new Agent { Name = "Mogwai", NameSort = "Mogwai" });
			_agentRepository.Create(new Agent { Name = "Mono Pause", NameSort = "Mono Pause" });
			_agentRepository.Create(new Agent { Name = "Moon Mullican", NameSort = "Mullican, Moon" });
			_agentRepository.Create(new Agent { Name = "Morwells", NameSort = "Morwells" });
			_agentRepository.Create(new Agent { Name = "Mose Se Sengo 'Fan Fan'", NameSort = "Mose Se Sengo 'Fan Fan'" });
			_agentRepository.Create(new Agent { Name = "Moses Asch", NameSort = "Asch, Moses" });
			_agentRepository.Create(new Agent { Name = "Motherlode", NameSort = "Motherlode" });
			_agentRepository.Create(new Agent { Name = "Mötorhead", NameSort = "Mötorhead" });
			_agentRepository.Create(new Agent { Name = "Muddy Waters", NameSort = "Muddy Waters" });
			_agentRepository.Create(new Agent { Name = "Mudhoney", NameSort = "Mudhoney" });
			_agentRepository.Create(new Agent { Name = "Mulatu Astatke", NameSort = "Astatke, Mulatu" });
			_agentRepository.Create(new Agent { Name = "Mutabaruka", NameSort = "Mutabaruka" });
			_agentRepository.Create(new Agent { Name = "Nana Anim Brempong I", NameSort = "Brempong I, Nana Anim" });
			_agentRepository.Create(new Agent { Name = "Nashville Pussy", NameSort = "Nashville Pussy" });
			_agentRepository.Create(new Agent { Name = "National Health", NameSort = "National Health" });
			_agentRepository.Create(new Agent { Name = "Ned Lagin", NameSort = "Lagin, Ned" });
			_agentRepository.Create(new Agent { Name = "Negativland", NameSort = "Negativland" });
			_agentRepository.Create(new Agent { Name = "Neil Wilburn", NameSort = "Wilburn, Neil" });
			_agentRepository.Create(new Agent { Name = "Neil Young", NameSort = "Young, Neil" });
			_agentRepository.Create(new Agent { Name = "Nels Cline Trio", NameSort = "Cline Trio, Nels", FileUnder = "Nels Cline" });
			_agentRepository.Create(new Agent { Name = "Nelson", NameSort = "Nelson" });
			_agentRepository.Create(new Agent { Name = "Nena", NameSort = "Nena" });
			_agentRepository.Create(new Agent { Name = "Nesuhi Ertegun", NameSort = "Ertegun, Nesuhi" });
			_agentRepository.Create(new Agent { Name = "Nethers", NameSort = "Nethers" });
			_agentRepository.Create(new Agent { Name = "Neu!", NameSort = "Neu!" });
			_agentRepository.Create(new Agent { Name = "Neville Brothers", NameSort = "Neville Brothers" });
			_agentRepository.Create(new Agent { Name = "New Order", NameSort = "New Order" });
			_agentRepository.Create(new Agent { Name = "New Radiant Storm King", NameSort = "New Radiant Storm King" });
			_agentRepository.Create(new Agent { Name = "New Riders of the Purple Sage", NameSort = "New Riders of the Purple Sage" });
			_agentRepository.Create(new Agent { Name = "Nick Gravenites", NameSort = "Gravenites, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Huggins", NameSort = "Huggins, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Launay", NameSort = "Launay, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Lowe", NameSort = "Lowe, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Luca", NameSort = "Luca, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Pickard", NameSort = "Pickard, Nick" });
			_agentRepository.Create(new Agent { Name = "Nick Robins", NameSort = "Robins, Nick" });
			_agentRepository.Create(new Agent { Name = "Nicky Hopkins", NameSort = "Hopkins, Nicky" });
			_agentRepository.Create(new Agent { Name = "Nicky Otis", NameSort = "Otis, Nicky" });
			_agentRepository.Create(new Agent { Name = "Nico", NameSort = "Nico" });
			_agentRepository.Create(new Agent { Name = "Nigel Godrich", NameSort = "Godrich, Nigel" });
			_agentRepository.Create(new Agent { Name = "Niko Bolas", NameSort = "Bolas, Niko" });
			_agentRepository.Create(new Agent { Name = "Nino Rota", NameSort = "Rota, Nino" });
			_agentRepository.Create(new Agent { Name = "Nirvana", NameSort = "Nirvana" });
			_agentRepository.Create(new Agent { Name = "Nkrumah \"Jah\" Thomas", NameSort = "Thomas, Nkrumah \"Jah\"" });
			_agentRepository.Create(new Agent { Name = "Noel Herne", NameSort = "Herne, Noel" });
			_agentRepository.Create(new Agent { Name = "Norman Grant", NameSort = "Grant, Norman" });
			_agentRepository.Create(new Agent { Name = "Nyboma et Bovi & Kilola", NameSort = "Nyboma et Bovi & Kilola" });
			_agentRepository.Create(new Agent { Name = "Of Montreal", NameSort = "Of Montreal" });
			_agentRepository.Create(new Agent { Name = "Old & In The Way", NameSort = "Old & In The Way" });
			_agentRepository.Create(new Agent { Name = "Oliver DiCicco", NameSort = "DiCicco, Oliver" });
			_agentRepository.Create(new Agent { Name = "Oral Roberts", NameSort = "Roberts, Oral" });
			_agentRepository.Create(new Agent { Name = "Orlann Divo", NameSort = "Divo, Orlann" });
			_agentRepository.Create(new Agent { Name = "Ornette Coleman", NameSort = "Coleman, Ornette" });
			_agentRepository.Create(new Agent { Name = "Oscar Peterson", NameSort = "Peterson, Oscar" });
			_agentRepository.Create(new Agent { Name = "Ossie Hibbert", NameSort = "Hibbert, Ossie" });
			_agentRepository.Create(new Agent { Name = "Oswald Palmer", NameSort = "Palmer, Oswald" });
			_agentRepository.Create(new Agent { Name = "Otis Redding", NameSort = "Redding, Otis" });
			_agentRepository.Create(new Agent { Name = "Otis Spann", NameSort = "Spann, Otis" });
			_agentRepository.Create(new Agent { Name = "Owsley Stanley", NameSort = "Stanley, Owsley" });
			_agentRepository.Create(new Agent { Name = "P. Funk All-Stars", NameSort = "P. Funk All-Stars" });
			_agentRepository.Create(new Agent { Name = "Paco DeLucia", NameSort = "DeLucia, Paco" });
			_agentRepository.Create(new Agent { Name = "Page Hamilton", NameSort = "Page Hamilton" });
			_agentRepository.Create(new Agent { Name = "Palace Brothers", NameSort = "Palace Brothers", FileUnder = "Will Oldham" });
			_agentRepository.Create(new Agent { Name = "Palace", NameSort = "Palace", FileUnder = "Will Oldham" });
			_agentRepository.Create(new Agent { Name = "Pappy Daily", NameSort = "Daily, Pappy" });
			_agentRepository.Create(new Agent { Name = "Parliament", NameSort = "Parliament" });
			_agentRepository.Create(new Agent { Name = "Pat Kelly", NameSort = "Kelly, Pat" });
			_agentRepository.Create(new Agent { Name = "Pat Martino", NameSort = "Martino, Pat" });
			_agentRepository.Create(new Agent { Name = "Pat Metheny", NameSort = "Metheny, Pat" });
			_agentRepository.Create(new Agent { Name = "Patrick B. Murphy", NameSort = "Murphy, Patrick B." });
			_agentRepository.Create(new Agent { Name = "Patsy Cline", NameSort = "Cline, Patsy" });
			_agentRepository.Create(new Agent { Name = "Paul Burch", NameSort = "Burch, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Goodman", NameSort = "Goodman, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Hamann", NameSort = "Hamann, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Hardiman", NameSort = "Hardiman, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Kantner", NameSort = "Kantner, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Kolderie", NameSort = "Kolderie, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul McCartney & Wings", NameSort = "McCartney & Wings, Paul", FileUnder = "Paul McCartney" });
			_agentRepository.Create(new Agent { Name = "Paul McCartney", NameSort = "McCartney, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Northfield", NameSort = "Northfield, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Revere & The Raiders", NameSort = "Revere & The Raiders, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Savage", NameSort = "Savage, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Simon", NameSort = "Simon, Paul" });
			_agentRepository.Create(new Agent { Name = "Paul Tipler", NameSort = "Tipler, Paul" });
			_agentRepository.Create(new Agent { Name = "Pavement", NameSort = "Pavement" });
			_agentRepository.Create(new Agent { Name = "Pax Nicholas and the Nettey Family", NameSort = "Nicholas and the Nettey Family, Pax" });
			_agentRepository.Create(new Agent { Name = "Pedro de Freitas", NameSort = "Pedro de Freitas" });
			_agentRepository.Create(new Agent { Name = "Peggy Lee", NameSort = "Lee, Peggy" });
			_agentRepository.Create(new Agent { Name = "Peggy Seeger", NameSort = "Seeger, Peggy" });
			_agentRepository.Create(new Agent { Name = "Pentangle", NameSort = "Pentangle" });
			_agentRepository.Create(new Agent { Name = "People Under The Stairs", NameSort = "People Under The Stairs" });
			_agentRepository.Create(new Agent { Name = "Pere Ubu", NameSort = "Pere Ubu" });
			_agentRepository.Create(new Agent { Name = "Perinho Albuquerque", NameSort = "Albuquerque, Perinho" });
			_agentRepository.Create(new Agent { Name = "Perry Farrell", NameSort = "Farrell, Perry" });
			_agentRepository.Create(new Agent { Name = "Pete Seeger", NameSort = "Seeger, Pete" });
			_agentRepository.Create(new Agent { Name = "Pete Weeding", NameSort = "Weeding, Pete" });
			_agentRepository.Create(new Agent { Name = "Peter Chemist", NameSort = "Chemist, Peter" });
			_agentRepository.Create(new Agent { Name = "Peter Frampton", NameSort = "Frampton, Peter" });
			_agentRepository.Create(new Agent { Name = "Peter Gabriel", NameSort = "Gabriel, Peter " });
			_agentRepository.Create(new Agent { Name = "Peter J. Walker", NameSort = "Walker, Peter J." });
			_agentRepository.Create(new Agent { Name = "Peter Kurland", NameSort = "Kurland, Peter" });
			_agentRepository.Create(new Agent { Name = "Peter Nichols", NameSort = "Nichols, Peter" });
			_agentRepository.Create(new Agent { Name = "Peter Paul", NameSort = "Paul, Peter" });
			_agentRepository.Create(new Agent { Name = "Peter Tosh", NameSort = "Tosh, Peter" });
			_agentRepository.Create(new Agent { Name = "Pharoah Sanders", NameSort = "Sanders, Pharoah" });
			_agentRepository.Create(new Agent { Name = "Phil Brown ", NameSort = "Brown, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Brown", NameSort = "Brown, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Ek", NameSort = "Ek, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Lesh", NameSort = "Lesh, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Macey", NameSort = "Macey, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Ochs", NameSort = "Ochs, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Ramone", NameSort = "Ramone, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Spector", NameSort = "Spector, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Thornalley", NameSort = "Thornalley, Phil" });
			_agentRepository.Create(new Agent { Name = "Phil Wright", NameSort = "Wright, Phil" });
			_agentRepository.Create(new Agent { Name = "Philip Smart", NameSort = "Smart, Philip" });
			_agentRepository.Create(new Agent { Name = "Phillip McDonald", NameSort = "McDonald, Phillip" });
			_agentRepository.Create(new Agent { Name = "Phillip Smart", NameSort = "Smart, Philip" });
			_agentRepository.Create(new Agent { Name = "Pierre Houon", NameSort = "Houon, Pierre" });
			_agentRepository.Create(new Agent { Name = "Pigpen", NameSort = "Pigpen" });
			_agentRepository.Create(new Agent { Name = "Pixies", NameSort = "Pixies" });
			_agentRepository.Create(new Agent { Name = "Plainsong", NameSort = "Plainsong" });
			_agentRepository.Create(new Agent { Name = "Pokey LaFarge and the South City Three", NameSort = "LaFarge and the South City Three, Pokey" });
			_agentRepository.Create(new Agent { Name = "Pokey LaFarge", NameSort = "LaFarge, Pokey" });
			_agentRepository.Create(new Agent { Name = "Ponga", NameSort = "Ponga" });
			_agentRepository.Create(new Agent { Name = "Pop Staples", NameSort = "Staples, Pop" });
			_agentRepository.Create(new Agent { Name = "Preston Love", NameSort = "Love, Preston" });
			_agentRepository.Create(new Agent { Name = "Preston School Of Industry", NameSort = "Preston School Of Industry" });
			_agentRepository.Create(new Agent { Name = "Pretenders", NameSort = "Pretenders" });
			_agentRepository.Create(new Agent { Name = "Pretty Purdie and the Playboys", NameSort = "Purdie and the Playboys, Pretty", FileUnder = "Bernard \"Pretty\" Purdie" });
			_agentRepository.Create(new Agent { Name = "Prince Alla", NameSort = "Prince Alla" });
			_agentRepository.Create(new Agent { Name = "Prince and the Revolution", NameSort = "Prince and the Revolution", FileUnder = "Prince" });
			_agentRepository.Create(new Agent { Name = "Prince Buster", NameSort = "Prince Buster" });
			_agentRepository.Create(new Agent { Name = "Prince Far I", NameSort = "Prince Far I" });
			_agentRepository.Create(new Agent { Name = "Prince Jammy", NameSort = "Prince Jammy" });
			_agentRepository.Create(new Agent { Name = "Prince Jazzbo", NameSort = "Prince Jazzbo" });
			_agentRepository.Create(new Agent { Name = "Prince Lasha", NameSort = "Prince Lasha" });
			_agentRepository.Create(new Agent { Name = "Prince Muhammed", NameSort = "Prince Muhammed" });
			_agentRepository.Create(new Agent { Name = "Prince Nico Mbarga", NameSort = "Prince Nico Mbarga" });
			_agentRepository.Create(new Agent { Name = "Prince Thony Adex", NameSort = "Prince Thony Adex" });
			_agentRepository.Create(new Agent { Name = "Prince", NameSort = "Prince" });
			_agentRepository.Create(new Agent { Name = "Professor Longhair", NameSort = "Professor Longhair" });
			_agentRepository.Create(new Agent { Name = "Public Image Ltd", NameSort = "Public Image Ltd" });
			_agentRepository.Create(new Agent { Name = "Purling Hiss", NameSort = "Purling Hiss" });
			_agentRepository.Create(new Agent { Name = "Pylon", NameSort = "Pylon" });
			_agentRepository.Create(new Agent { Name = "Quasi", NameSort = "Quasi" });
			_agentRepository.Create(new Agent { Name = "Queen", NameSort = "Queen" });
			_agentRepository.Create(new Agent { Name = "Quicksilver Messenger Service", NameSort = "Quicksilver Messenger Service" });
			_agentRepository.Create(new Agent { Name = "Quincy Jones & His Orchestra", NameSort = "Jones & His Orchestra, Quincy", FileUnder = "Quincy Jones" });
			_agentRepository.Create(new Agent { Name = "Quincy Jones", NameSort = "Jones, Quincy" });
			_agentRepository.Create(new Agent { Name = "R.E.M.", NameSort = "R.E.M." });
			_agentRepository.Create(new Agent { Name = "Radiohead", NameSort = "Radiohead" });
			_agentRepository.Create(new Agent { Name = "Raghu", NameSort = "Raghu" });
			_agentRepository.Create(new Agent { Name = "Rail-Band Orchestra of Bamako", NameSort = "Rail-Band Orchestra of Bamako" });
			_agentRepository.Create(new Agent { Name = "Ramones", NameSort = "Ramones" });
			_agentRepository.Create(new Agent { Name = "Randy Kling", NameSort = "Kling, Randy" });
			_agentRepository.Create(new Agent { Name = "Randy Newman", NameSort = "Newman, Randy" });
			_agentRepository.Create(new Agent { Name = "Randy Phipps", NameSort = "Phipps, Randy" });
			_agentRepository.Create(new Agent { Name = "Ranking Joe", NameSort = "Ranking Joe" });
			_agentRepository.Create(new Agent { Name = "Rare Earth", NameSort = "Rare Earth" });
			_agentRepository.Create(new Agent { Name = "Ras Michael & The Sons of Negus", NameSort = "Ras Michael & The Sons of Negus", FileUnder = "Ras Michael" });
			_agentRepository.Create(new Agent { Name = "Ras Michael", NameSort = "Ras Michael" });
			_agentRepository.Create(new Agent { Name = "Ravi Shankar", NameSort = "Ravi Shankar" });
			_agentRepository.Create(new Agent { Name = "Ray Davies", NameSort = "Davies, Ray" });
			_agentRepository.Create(new Agent { Name = "Ray Manzarek", NameSort = "Manzarek, Ray" });
			_agentRepository.Create(new Agent { Name = "Ray Price", NameSort = "Price, Ray" });
			_agentRepository.Create(new Agent { Name = "Ray Thomas Baker", NameSort = "Baker, Ray Thomas" });
			_agentRepository.Create(new Agent { Name = "Red Aunts", NameSort = "Red Aunts" });
			_agentRepository.Create(new Agent { Name = "Red Fang", NameSort = "Red Fang" });
			_agentRepository.Create(new Agent { Name = "Red Rockers", NameSort = "Red Rockers" });
			_agentRepository.Create(new Agent { Name = "Reginal Foort", NameSort = "Foort, Reginal" });
			_agentRepository.Create(new Agent { Name = "Remmy Ongala & Orchestre Super Matimila", NameSort = "Ongala & Orchestre Super Matimila, Remmy" });
			_agentRepository.Create(new Agent { Name = "Renaissance", NameSort = "Renaissance" });
			_agentRepository.Create(new Agent { Name = "Reo Edwards", NameSort = "Edwards, Reo" });
			_agentRepository.Create(new Agent { Name = "Reverand Gary Davis", NameSort = "Davis, Reverand Gary" });
			_agentRepository.Create(new Agent { Name = "Revolutionaries", NameSort = "Revolutionaries" });
			_agentRepository.Create(new Agent { Name = "Rex Updegraft", NameSort = "Updegraft, Rex" });
			_agentRepository.Create(new Agent { Name = "Rhett Davies", NameSort = "Davies, Rhett" });
			_agentRepository.Create(new Agent { Name = "Rich Costey", NameSort = "Costey, Rich" });
			_agentRepository.Create(new Agent { Name = "Richard & Linda Thompson", NameSort = "Thompson, Richard & Linda" });
			_agentRepository.Create(new Agent { Name = "Richard \"Groove\" Holmes", NameSort = "Holmes, Richard \"Groove\"" });
			_agentRepository.Create(new Agent { Name = "Richard Finch", NameSort = "Finch, Richard" });
			_agentRepository.Create(new Agent { Name = "Richard James Burgess", NameSort = "Burgess, Richard James" });
			_agentRepository.Create(new Agent { Name = "Richard Pryor", NameSort = "Pryor, Richard" });
			_agentRepository.Create(new Agent { Name = "Richard Rodgers", NameSort = "Rodgers, Richard" });
			_agentRepository.Create(new Agent { Name = "Richard Thompson", NameSort = "Thompson, Richard" });
			_agentRepository.Create(new Agent { Name = "Rick Hall", NameSort = "Hall, Rick" });
			_agentRepository.Create(new Agent { Name = "Rick Rubin", NameSort = "Rubin, Rick" });
			_agentRepository.Create(new Agent { Name = "Rick Wakeman", NameSort = "Wakeman, Rick" });
			_agentRepository.Create(new Agent { Name = "Rickie Lee Jones", NameSort = "Jones, Rickie Lee" });
			_agentRepository.Create(new Agent { Name = "Ricky Skaggs", NameSort = "Skaggs, Ricky" });
			_agentRepository.Create(new Agent { Name = "Rik Grech", NameSort = "Grech, Rik" });
			_agentRepository.Create(new Agent { Name = "Riley & Dennis", NameSort = "Riley & Dennis" });
			_agentRepository.Create(new Agent { Name = "Rob Cable", NameSort = "Cable, Rob" });
			_agentRepository.Create(new Agent { Name = "Rob Eaton", NameSort = "Eaton, Rob" });
			_agentRepository.Create(new Agent { Name = "Rob Fraboni", NameSort = "Fraboni, Rob" });
			_agentRepository.Create(new Agent { Name = "Robb Di Stefano", NameSort = "Di Stefano, Robb" });
			_agentRepository.Create(new Agent { Name = "Robert Both", NameSort = "Both, Robert" });
			_agentRepository.Create(new Agent { Name = "Robert G. Koester", NameSort = "Koester, Robert G." });
			_agentRepository.Create(new Agent { Name = "Robert Hunter", NameSort = "Hunter, Robert" });
			_agentRepository.Create(new Agent { Name = "Robert Jackson", NameSort = "Jackson, Robert" });
			_agentRepository.Create(new Agent { Name = "Robert John Lange", NameSort = "Lange, Robert John" });
			_agentRepository.Create(new Agent { Name = "Robert Nicholson", NameSort = "Nicholson, Robert" });
			_agentRepository.Create(new Agent { Name = "Robert Palmer", NameSort = "Palmer, Robert" });
			_agentRepository.Create(new Agent { Name = "Robert Takoushian", NameSort = "Takoushian, Robert" });
			_agentRepository.Create(new Agent { Name = "Roberto Menescal", NameSort = "Menescal, Roberto" });
			_agentRepository.Create(new Agent { Name = "Robin Hitchcock & The Egyptians", NameSort = "Hitchcock & The Egyptians, Robin", FileUnder = "Robin Hitchcock" });
			_agentRepository.Create(new Agent { Name = "Rod Stewart", NameSort = "Stewart, Rod" });
			_agentRepository.Create(new Agent { Name = "Rod Taylor", NameSort = "Taylor, Rod" });
			_agentRepository.Create(new Agent { Name = "Rodger Bain", NameSort = "Bain, Rodger" });
			_agentRepository.Create(new Agent { Name = "Rodney Dangerfield", NameSort = "Dangerfield, Rodney" });
			_agentRepository.Create(new Agent { Name = "Roger Beale", NameSort = "Beale, Roger" });
			_agentRepository.Create(new Agent { Name = "Roger Bechirian", NameSort = "Bechirian, Roger" });
			_agentRepository.Create(new Agent { Name = "Roger McGuinn", NameSort = "McGuinn, Roger" });
			_agentRepository.Create(new Agent { Name = "Roger Moutenot", NameSort = "Moutenot, Roger" });
			_agentRepository.Create(new Agent { Name = "Roger Wake", NameSort = "Wake, Roger" });
			_agentRepository.Create(new Agent { Name = "Roger", NameSort = "Roger" });
			_agentRepository.Create(new Agent { Name = "Rohan Dwyer", NameSort = "Dwyer, Rohan" });
			_agentRepository.Create(new Agent { Name = "Roland Alphonso", NameSort = "Alphonso, Roland" });
			_agentRepository.Create(new Agent { Name = "Ron Albert", NameSort = "Albert, Ron" });
			_agentRepository.Create(new Agent { Name = "Ron Capone", NameSort = "Capone, Ron" });
			_agentRepository.Create(new Agent { Name = "Ron Mael", NameSort = "Mael, Ron" });
			_agentRepository.Create(new Agent { Name = "Ron St. Germain", NameSort = "St. Germain, Ron" });
			_agentRepository.Create(new Agent { Name = "Ron Terry", NameSort = "Terry, Ron" });
			_agentRepository.Create(new Agent { Name = "Ron Wickersham", NameSort = "Wickersham, Ron" });
			_agentRepository.Create(new Agent { Name = "Ronnie Albert", NameSort = "Albert, Ronnie" });
			_agentRepository.Create(new Agent { Name = "Ronnie Lane", NameSort = "Lane, Ronnie" });
			_agentRepository.Create(new Agent { Name = "Roots & Wailers Band", NameSort = "Roots & Wailers Band" });
			_agentRepository.Create(new Agent { Name = "Roots Radics Band", NameSort = "Roots Radics Band" });
			_agentRepository.Create(new Agent { Name = "Roots Radics", NameSort = "Roots Radics" });
			_agentRepository.Create(new Agent { Name = "Roy Burrows", NameSort = "Burrows, Roy" });
			_agentRepository.Create(new Agent { Name = "Roy Cicala", NameSort = "Cicala, Roy" });
			_agentRepository.Create(new Agent { Name = "Roy Homer", NameSort = "Homer, Roy" });
			_agentRepository.Create(new Agent { Name = "Roy Orbison", NameSort = "Orbison, Roy" });
			_agentRepository.Create(new Agent { Name = "Roy Reid", NameSort = "Reid, Roy" });
			_agentRepository.Create(new Agent { Name = "Roy Wood", NameSort = "Wood, Roy" });
			_agentRepository.Create(new Agent { Name = "Royal Trux", NameSort = "Royal Trux" });
			_agentRepository.Create(new Agent { Name = "Rudy Van Gelder", NameSort = "Van Gelder, Rudy" });
			_agentRepository.Create(new Agent { Name = "Run-D.M.C.", NameSort = "Run-D.M.C." });
			_agentRepository.Create(new Agent { Name = "Rupert Clemendore", NameSort = "Clemendore, Rupert" });
			_agentRepository.Create(new Agent { Name = "Russell Mael", NameSort = "Mael, Russell" });
			_agentRepository.Create(new Agent { Name = "Russell Simmons", NameSort = "Simmons, Russell" });
			_agentRepository.Create(new Agent { Name = "Ruth Welcome", NameSort = "Welcome, Ruth" });
			_agentRepository.Create(new Agent { Name = "Ry Cooder", NameSort = "Cooder, Ry" });
			_agentRepository.Create(new Agent { Name = "Ryuichi Sakamoto", NameSort = "Sakamoto, Ryuichi " });
			_agentRepository.Create(new Agent { Name = "Ryuichi Sakamoto", NameSort = "Sakamoto, Ryuichi" });
			_agentRepository.Create(new Agent { Name = "Sadhappy", NameSort = "Sadhappy" });
			_agentRepository.Create(new Agent { Name = "Salem 66", NameSort = "Salem 66" });
			_agentRepository.Create(new Agent { Name = "Sam Mangwana", NameSort = "Mangwana, Sam" });
			_agentRepository.Create(new Agent { Name = "Samuel Charters", NameSort = "Charters, Samuel" });
			_agentRepository.Create(new Agent { Name = "Sandy Denny", NameSort = "Denny, Sandy" });
			_agentRepository.Create(new Agent { Name = "Sandy Fisher", NameSort = "Fisher, Sandy" });
			_agentRepository.Create(new Agent { Name = "Santana", NameSort = "Santana" });
			_agentRepository.Create(new Agent { Name = "Sarah Lee Guthrie", NameSort = "Guthrie, Sarah Lee" });
			_agentRepository.Create(new Agent { Name = "Scientist ", NameSort = "Scientist " });
			_agentRepository.Create(new Agent { Name = "Scientist", NameSort = "Scientist" });
			_agentRepository.Create(new Agent { Name = "Scott Billington", NameSort = "Billington, Scott" });
			_agentRepository.Create(new Agent { Name = "Scott Bomar", NameSort = "Bomar, Scott" });
			_agentRepository.Create(new Agent { Name = "Scott English", NameSort = "English, Scott" });
			_agentRepository.Create(new Agent { Name = "Scott Joplin", NameSort = "Joplin, Scott" });
			_agentRepository.Create(new Agent { Name = "Scott Litt", NameSort = "Litt, Scott" });
			_agentRepository.Create(new Agent { Name = "Screamin' Jay Hawkins", NameSort = "Hawkins, Screamin' Jay" });
			_agentRepository.Create(new Agent { Name = "Screaming Trees", NameSort = "Screaming Trees" });
			_agentRepository.Create(new Agent { Name = "Sean Slade", NameSort = "Slade, Sean" });
			_agentRepository.Create(new Agent { Name = "Sebadoh", NameSort = "Sebadoh" });
			_agentRepository.Create(new Agent { Name = "Sekou Coulibaly", NameSort = "Coulibaly, Sekou" });
			_agentRepository.Create(new Agent { Name = "Sergio Mendes & Brasil 66", NameSort = "Mendes & Brasil 66, Sergio" });
			_agentRepository.Create(new Agent { Name = "Shakti With John McLaughlin", NameSort = "Shakti With John McLaughlin", FileUnder = "John McLaughlin" });
			_agentRepository.Create(new Agent { Name = "Sharon Jones & The Dap Kings", NameSort = "Jones & The Dap Kings, Sharon" });
			_agentRepository.Create(new Agent { Name = "Shaun Herbert", NameSort = "Herbert, Shaun" });
			_agentRepository.Create(new Agent { Name = "Shelly Yakus", NameSort = "Yakus, Shelly" });
			_agentRepository.Create(new Agent { Name = "Shinehead", NameSort = "Shinehead" });
			_agentRepository.Create(new Agent { Name = "Shirley Bassie", NameSort = "Bassie, Shirley" });
			_agentRepository.Create(new Agent { Name = "Sholem Asch", NameSort = "Asch, Sholem" });
			_agentRepository.Create(new Agent { Name = "Shortstack", NameSort = "Shortstack" });
			_agentRepository.Create(new Agent { Name = "Shuggie Otis", NameSort = "Otis, Shuggie" });
			_agentRepository.Create(new Agent { Name = "Si Waronker", NameSort = "Waronker, Si" });
			_agentRepository.Create(new Agent { Name = "Sigur Ros", NameSort = "Sigur Ros" });
			_agentRepository.Create(new Agent { Name = "Silkworm", NameSort = "Silkworm" });
			_agentRepository.Create(new Agent { Name = "Silver Jews", NameSort = "Silver Jews" });
			_agentRepository.Create(new Agent { Name = "Simon and Garfunkel", NameSort = "Simon and Garfunkel" });
			_agentRepository.Create(new Agent { Name = "Simon Nicol", NameSort = "Nicol, Simon" });
			_agentRepository.Create(new Agent { Name = "Siouxsie and the Banshees", NameSort = "Siouxsie and the Banshees" });
			_agentRepository.Create(new Agent { Name = "Sir Douglas Quintet", NameSort = "Sir Douglas Quintet" });
			_agentRepository.Create(new Agent { Name = "Sister Nancy", NameSort = "Sister Nancy" });
			_agentRepository.Create(new Agent { Name = "Slint", NameSort = "Slint" });
			_agentRepository.Create(new Agent { Name = "Slow Poke", NameSort = "Slow Poke" });
			_agentRepository.Create(new Agent { Name = "Slug", NameSort = "Slug" });
			_agentRepository.Create(new Agent { Name = "Sly & Robbie ", NameSort = "Sly & Robbie " });
			_agentRepository.Create(new Agent { Name = "Sly & Robbie", NameSort = "Sly & Robbie" });
			_agentRepository.Create(new Agent { Name = "Sly and the Family Stone", NameSort = "Sly and the Family Stone" });
			_agentRepository.Create(new Agent { Name = "Sly Stone", NameSort = "Sly Stone" });
			_agentRepository.Create(new Agent { Name = "Small Faces", NameSort = "Small Faces" });
			_agentRepository.Create(new Agent { Name = "Social Distortion", NameSort = "Social Distortion" });
			_agentRepository.Create(new Agent { Name = "Soft Circle", NameSort = "Soft Circle" });
			_agentRepository.Create(new Agent { Name = "Soft Machine", NameSort = "Soft Machine" });
			_agentRepository.Create(new Agent { Name = "Sojie", NameSort = "Sojie" });
			_agentRepository.Create(new Agent { Name = "Soldgie", NameSort = "Soldgie" });
			_agentRepository.Create(new Agent { Name = "Soljie", NameSort = "Soljie" });
			_agentRepository.Create(new Agent { Name = "Solomon Ilori & His Afro-Drum Ensemble", NameSort = "Ilori & His Afro-Drum Ensemble, Solomon" });
			_agentRepository.Create(new Agent { Name = "Somo Somo", NameSort = "Somo Somo", FileUnder = "Mose Se Sengo 'Fan Fan'" });
			_agentRepository.Create(new Agent { Name = "Sonic Youth", NameSort = "Sonic Youth" });
			_agentRepository.Create(new Agent { Name = "Sonic Youth", NameSort = "Sonic Youth", FileUnder = "Mose Se Sengo 'Fan Fan'" });
			_agentRepository.Create(new Agent { Name = "Sonny Boy Williamson", NameSort = "Williamson, Sonny Boy" });
			_agentRepository.Create(new Agent { Name = "Sonny Lester", NameSort = "Lester, Sonny" });
			_agentRepository.Create(new Agent { Name = "Soraya Melik", NameSort = "Melik, Soraya" });
			_agentRepository.Create(new Agent { Name = "Soul Asylum", NameSort = "Soul Asylum" });
			_agentRepository.Create(new Agent { Name = "Soul Syndicate", NameSort = "Soul Syndicate" });
			_agentRepository.Create(new Agent { Name = "Soul Vendors", NameSort = "Soul Vendors" });
			_agentRepository.Create(new Agent { Name = "Sound Dimension", NameSort = "Sound Dimension" });
			_agentRepository.Create(new Agent { Name = "Soundgarden", NameSort = "Soundgarden" });
			_agentRepository.Create(new Agent { Name = "Space Pants", NameSort = "Space Pants" });
			_agentRepository.Create(new Agent { Name = "Sparks", NameSort = "Sparks" });
			_agentRepository.Create(new Agent { Name = "Spencer Davis Group", NameSort = "Spencer Davis Group" });
			_agentRepository.Create(new Agent { Name = "Spiral Stairs", NameSort = "Spiral Stairs" });
			_agentRepository.Create(new Agent { Name = "Spiritualized", NameSort = "Spiritualized" });
			_agentRepository.Create(new Agent { Name = "Spoon", NameSort = "Spoon" });
			_agentRepository.Create(new Agent { Name = "Spot", NameSort = "Spot" });
			_agentRepository.Create(new Agent { Name = "Squeeze", NameSort = "Squeeze" });
			_agentRepository.Create(new Agent { Name = "Stan Lee", NameSort = "Lee, Stan" });
			_agentRepository.Create(new Agent { Name = "Steel Pulse", NameSort = "Steel Pulse" });
			_agentRepository.Create(new Agent { Name = "Steely Dan", NameSort = "Steely Dan" });
			_agentRepository.Create(new Agent { Name = "Stefan Bright", NameSort = "Bright, Stefan" });
			_agentRepository.Create(new Agent { Name = "Stephane Grappelli", NameSort = "Grappelli, Stephane" });
			_agentRepository.Create(new Agent { Name = "Stephen Malkmus", NameSort = "Malkmus, Stephen" });
			_agentRepository.Create(new Agent { Name = "Stephen Stills", NameSort = "Stills, Stephen" });
			_agentRepository.Create(new Agent { Name = "Stereolab", NameSort = "Stereolab" });
			_agentRepository.Create(new Agent { Name = "Steve Albini", NameSort = "Albini, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Bamcard", NameSort = "Bamcard, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Cropper", NameSort = "Cropper, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Fisk", NameSort = "Fisk, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Gaboury", NameSort = "Gaboury, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Lillywhite", NameSort = "Lillywhite, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Marriott", NameSort = "Marriott, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Martin", NameSort = "Martin, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Melton", NameSort = "Melton, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Rathe", NameSort = "Rathe, Steve" });
			_agentRepository.Create(new Agent { Name = "Steve Smith", NameSort = "Smith, Steve" });
			_agentRepository.Create(new Agent { Name = "Steven Fjelstad", NameSort = "Fjelstad, Steven" });
			_agentRepository.Create(new Agent { Name = "Steven Stanley", NameSort = "Stanley, Steven" });
			_agentRepository.Create(new Agent { Name = "Stevie Wonder", NameSort = "Wonder, Stevie" });
			_agentRepository.Create(new Agent { Name = "Stiff Little Fingers", NameSort = "Stiff Little Fingers" });
			_agentRepository.Create(new Agent { Name = "Stuart Hallerman", NameSort = "Hallerman, Stuart" });
			_agentRepository.Create(new Agent { Name = "Sugar Hill Gang", NameSort = "Sugar Hill Gang" });
			_agentRepository.Create(new Agent { Name = "Sugar", NameSort = "Sugar" });
			_agentRepository.Create(new Agent { Name = "Suge Knight", NameSort = "Knight, Suge" });
			_agentRepository.Create(new Agent { Name = "Sun City Girls", NameSort = "Sun City Girls" });
			_agentRepository.Create(new Agent { Name = "Sun Ra", NameSort = "Sun Ra" });
			_agentRepository.Create(new Agent { Name = "Sunwolf", NameSort = "Sunwolf" });
			_agentRepository.Create(new Agent { Name = "Suzanne Vega", NameSort = "Vega, Suzanne" });
			_agentRepository.Create(new Agent { Name = "Swimming Pool Q's", NameSort = "Swimming Pool Q's" });
			_agentRepository.Create(new Agent { Name = "Sylvan Morris", NameSort = "Morris, Sylvan" });
			_agentRepository.Create(new Agent { Name = "Sylvester Stewart", NameSort = "Stewart, Sylvester" });
			_agentRepository.Create(new Agent { Name = "T Bone Burnett", NameSort = "Burnett, T Bone" });
			_agentRepository.Create(new Agent { Name = "T. Erdelyi", NameSort = "Erdelyi, T." });
			_agentRepository.Create(new Agent { Name = "T.S. Eliot", NameSort = "Eliot, T.S." });
			_agentRepository.Create(new Agent { Name = "Tabu Ley Rochereau", NameSort = "Rochereau, Tabu Ley" });
			_agentRepository.Create(new Agent { Name = "Tad", NameSort = "Tad" });
			_agentRepository.Create(new Agent { Name = "Taj Mahal", NameSort = "Taj Mahal" });
			_agentRepository.Create(new Agent { Name = "Talking Heads", NameSort = "Talking Heads" });
			_agentRepository.Create(new Agent { Name = "Talulah Gosh", NameSort = "Talulah Gosh" });
			_agentRepository.Create(new Agent { Name = "Tanya Stephens", NameSort = "Stephens, Tanya" });
			_agentRepository.Create(new Agent { Name = "Tara Jane Oneil", NameSort = "Oneil, Tara Jane" });
			_agentRepository.Create(new Agent { Name = "Ted Brosman", NameSort = "Brosman, Ted" });
			_agentRepository.Create(new Agent { Name = "Teddy Reig", NameSort = "Reig, Teddy" });
			_agentRepository.Create(new Agent { Name = "Tee", NameSort = "Tee" });
			_agentRepository.Create(new Agent { Name = "Television", NameSort = "Television" });
			_agentRepository.Create(new Agent { Name = "Teo Macero", NameSort = "Macero, Teo" });
			_agentRepository.Create(new Agent { Name = "Terry Katzman", NameSort = "Katzman, Terry" });
			_agentRepository.Create(new Agent { Name = "Terry Knight", NameSort = "Knight, Terry" });
			_agentRepository.Create(new Agent { Name = "Terry Melcher", NameSort = "Melcher, Terry" });
			_agentRepository.Create(new Agent { Name = "That Petrol Emotion", NameSort = "That Petrol Emotion" });
			_agentRepository.Create(new Agent { Name = "The Aggrovators", NameSort = "Aggrovators, The" });
			_agentRepository.Create(new Agent { Name = "The Alarm", NameSort = "Alarm, The" });
			_agentRepository.Create(new Agent { Name = "The Allman Brothers Band", NameSort = "Allman Brothers Band, The" });
			_agentRepository.Create(new Agent { Name = "The Anomoanon", NameSort = "Anomoanon, The" });
			_agentRepository.Create(new Agent { Name = "The B-52's", NameSort = "B-52's, The" });
			_agentRepository.Create(new Agent { Name = "The Band", NameSort = "Band, The" });
			_agentRepository.Create(new Agent { Name = "The Bassies", NameSort = "Bassies, The" });
			_agentRepository.Create(new Agent { Name = "The Beach Boys", NameSort = "Beach Boys, The" });
			_agentRepository.Create(new Agent { Name = "The Beatles", NameSort = "Beatles, The" });
			_agentRepository.Create(new Agent { Name = "The Blackbyrds", NameSort = "Blackbyrds, The" });
			_agentRepository.Create(new Agent { Name = "The Blues Project", NameSort = "Blues Project, The" });
			_agentRepository.Create(new Agent { Name = "The Boarding Party", NameSort = "Boarding Party, The" });
			_agentRepository.Create(new Agent { Name = "The Budos Band", NameSort = "Budos Band, The" });
			_agentRepository.Create(new Agent { Name = "The Byrds", NameSort = "Byrds, The" });
			_agentRepository.Create(new Agent { Name = "The Cables", NameSort = "Cables, The" });
			_agentRepository.Create(new Agent { Name = "The Cannonball Adderley Quintet", NameSort = "Cannonball Adderley Quintet, The" });
			_agentRepository.Create(new Agent { Name = "The Carlsonics", NameSort = "Carlsonics, The" });
			_agentRepository.Create(new Agent { Name = "The Cars", NameSort = "Cars, The" });
			_agentRepository.Create(new Agent { Name = "The Charles Lloyd Quartet", NameSort = "Charles Lloyd Quartet, The" });
			_agentRepository.Create(new Agent { Name = "The Chicago Bears", NameSort = "Chicago Bears, The" });
			_agentRepository.Create(new Agent { Name = "The Chieftains", NameSort = "Chieftains, The" });
			_agentRepository.Create(new Agent { Name = "The Chi-Lites", NameSort = "Chi-Lites, The" });
			_agentRepository.Create(new Agent { Name = "The City Champs", NameSort = "City Champs, The" });
			_agentRepository.Create(new Agent { Name = "The Clash", NameSort = "Clash, The" });
			_agentRepository.Create(new Agent { Name = "The Colour Field", NameSort = "Colour Field, The" });
			_agentRepository.Create(new Agent { Name = "The Congos & Friends", NameSort = "Congos & Friends, The" });
			_agentRepository.Create(new Agent { Name = "The Congos", NameSort = "Congos, The" });
			_agentRepository.Create(new Agent { Name = "The Creation", NameSort = "Creation, The" });
			_agentRepository.Create(new Agent { Name = "The Cribs", NameSort = "Cribs, The" });
			_agentRepository.Create(new Agent { Name = "The Crystals", NameSort = "Crystals, The" });
			_agentRepository.Create(new Agent { Name = "The Cure", NameSort = "Cure, The" });
			_agentRepository.Create(new Agent { Name = "The Dandy Warhols", NameSort = "Dandy Warhols, The" });
			_agentRepository.Create(new Agent { Name = "The Dave Clark Five", NameSort = "Dave Clark Five, The" });
			_agentRepository.Create(new Agent { Name = "The David Grisman Quintet", NameSort = "David Grisman Quintet, The" });
			_agentRepository.Create(new Agent { Name = "The DB's", NameSort = "DB's, The" });
			_agentRepository.Create(new Agent { Name = "The Dickies", NameSort = "Dickies, The" });
			_agentRepository.Create(new Agent { Name = "The Dominatrix", NameSort = "Dominatrix, The" });
			_agentRepository.Create(new Agent { Name = "The Doobie Brothers", NameSort = "Doobie Brothers, The" });
			_agentRepository.Create(new Agent { Name = "The Doors", NameSort = "Doors, The" });
			_agentRepository.Create(new Agent { Name = "The Dukes of Stratosphear", NameSort = "Dukes of Stratosphear, The" });
			_agentRepository.Create(new Agent { Name = "The Dynamites", NameSort = "Dynamites, The" });
			_agentRepository.Create(new Agent { Name = "The English Beat", NameSort = "English Beat, The" });
			_agentRepository.Create(new Agent { Name = "The Entourage Music and Theatre Ensemble", NameSort = "Entourage Music and Theatre Ensemble, The" });
			_agentRepository.Create(new Agent { Name = "The Fall", NameSort = "Fall, The" });
			_agentRepository.Create(new Agent { Name = "The Flying Burrito Brothers", NameSort = "Flying Burrito Brothers, The" });
			_agentRepository.Create(new Agent { Name = "The Fourth Way", NameSort = "Fourth Way, The" });
			_agentRepository.Create(new Agent { Name = "The Friends of Distinction", NameSort = "Friends of Distinction, The" });
			_agentRepository.Create(new Agent { Name = "The Grateful Dead", NameSort = "Grateful Dead, The" });
			_agentRepository.Create(new Agent { Name = "The Groop", NameSort = "The Groop" });
			_agentRepository.Create(new Agent { Name = "The Halo Benders", NameSort = "Halo Benders, The" });
			_agentRepository.Create(new Agent { Name = "The Heptones", NameSort = "Heptones, The" });
			_agentRepository.Create(new Agent { Name = "The Housemartins", NameSort = "Housemartins, The" });
			_agentRepository.Create(new Agent { Name = "The Impressions", NameSort = "Impressions, The" });
			_agentRepository.Create(new Agent { Name = "The Incredible String Band", NameSort = "Incredible String Band, The" });
			_agentRepository.Create(new Agent { Name = "The Isley Brothers", NameSort = "Isley Brothers, The" });
			_agentRepository.Create(new Agent { Name = "The Itals", NameSort = "Itals, The" });
			_agentRepository.Create(new Agent { Name = "The J.B.'s", NameSort = "J.B.'s, The" });
			_agentRepository.Create(new Agent { Name = "The Jam", NameSort = "Jam, The" });
			_agentRepository.Create(new Agent { Name = "The Jesus And Mary Chain", NameSort = "Jesus And Mary Chain, The" });
			_agentRepository.Create(new Agent { Name = "The John Handy Quintet", NameSort = "John Handy Quintet, The" });
			_agentRepository.Create(new Agent { Name = "The John Lurie National Orchestra", NameSort = "John Lurie National Orchestra, The" });
			_agentRepository.Create(new Agent { Name = "The King Khan & BBQ Show", NameSort = "King Khan & BBQ Show, The" });
			_agentRepository.Create(new Agent { Name = "The Kinks", NameSort = "Kinks, The" });
			_agentRepository.Create(new Agent { Name = "The Lone Ranger", NameSort = "Lone Ranger, The" });
			_agentRepository.Create(new Agent { Name = "The Lounge Lizards", NameSort = "Lounge Lizards, The" });
			_agentRepository.Create(new Agent { Name = "The Mar-Keys", NameSort = "Mar-Keys, The " });
			_agentRepository.Create(new Agent { Name = "The Meters", NameSort = "Meters, The" });
			_agentRepository.Create(new Agent { Name = "The Mighty Diamonds", NameSort = "Mighty Diamonds, The" });
			_agentRepository.Create(new Agent { Name = "The Moonglows", NameSort = "Moonglows, The" });
			_agentRepository.Create(new Agent { Name = "The Move", NameSort = "Move, The" });
			_agentRepository.Create(new Agent { Name = "The New Vaudeville Band", NameSort = "New Vaudeville Band, The" });
			_agentRepository.Create(new Agent { Name = "The Nice", NameSort = "Nice, The" });
			_agentRepository.Create(new Agent { Name = "The Norman Luboff Choir", NameSort = "Norman Luboff Choir, The" });
			_agentRepository.Create(new Agent { Name = "The Observer Allstars", NameSort = "Observer Allstars, The" });
			_agentRepository.Create(new Agent { Name = "The Officials", NameSort = "Officials, The" });
			_agentRepository.Create(new Agent { Name = "The Olympia Brass Band of New Orleans", NameSort = "Olympia Brass Band of New Orleans, The" });
			_agentRepository.Create(new Agent { Name = "The Paragons", NameSort = "Paragons, The" });
			_agentRepository.Create(new Agent { Name = "The Pogues And The Dubliners", NameSort = "Pogues And The Dubliners, The" });
			_agentRepository.Create(new Agent { Name = "The Police", NameSort = "Police, The" });
			_agentRepository.Create(new Agent { Name = "The Postal Service", NameSort = "Postal Service, The" });
			_agentRepository.Create(new Agent { Name = "The Pretenders", NameSort = "Pretenders, The" });
			_agentRepository.Create(new Agent { Name = "The Professionals", NameSort = "Professionals, The" });
			_agentRepository.Create(new Agent { Name = "The Psychedelic Furs", NameSort = "Psychedelic Furs, The" });
			_agentRepository.Create(new Agent { Name = "The Replacements", NameSort = "Replacements, The" });
			_agentRepository.Create(new Agent { Name = "The Revolutionaries", NameSort = "Revolutionaries, The" });
			_agentRepository.Create(new Agent { Name = "The Rolling Stones", NameSort = "Rolling Stones, The" });
			_agentRepository.Create(new Agent { Name = "The Scene", NameSort = "Scene, The" });
			_agentRepository.Create(new Agent { Name = "The Sea And Cake", NameSort = "Sea And Cake, The" });
			_agentRepository.Create(new Agent { Name = "The Seldom Scene", NameSort = "Seldom Scene, The" });
			_agentRepository.Create(new Agent { Name = "The Selecter", NameSort = "Selecter, The" });
			_agentRepository.Create(new Agent { Name = "The Shins", NameSort = "Shins, The" });
			_agentRepository.Create(new Agent { Name = "The Skatalites", NameSort = "Skatalites, The" });
			_agentRepository.Create(new Agent { Name = "The Smiths", NameSort = "Smiths, The" });
			_agentRepository.Create(new Agent { Name = "The Sonny Clark Memorial Quartet", NameSort = "Sonny Clark Memorial Quartet, The" });
			_agentRepository.Create(new Agent { Name = "The Soul Brothers", NameSort = "Soul Brothers, The" });
			_agentRepository.Create(new Agent { Name = "The Special AKA", NameSort = "Special AKA, The" });
			_agentRepository.Create(new Agent { Name = "The Specials", NameSort = "Specials, The" });
			_agentRepository.Create(new Agent { Name = "The Squalls", NameSort = "Squalls, The" });
			_agentRepository.Create(new Agent { Name = "The Staple Singers", NameSort = "Staple Singers, The" });
			_agentRepository.Create(new Agent { Name = "The Stranglers", NameSort = "Stranglers, The" });
			_agentRepository.Create(new Agent { Name = "The Style Council", NameSort = "Style Council, The" });
			_agentRepository.Create(new Agent { Name = "The Sugarcubes", NameSort = "Sugarcubes, The" });
			_agentRepository.Create(new Agent { Name = "The Sundays", NameSort = "Sundays, The" });
			_agentRepository.Create(new Agent { Name = "The Supremes", NameSort = "Supremes, The" });
			_agentRepository.Create(new Agent { Name = "The Tenors", NameSort = "Tenors, The" });
			_agentRepository.Create(new Agent { Name = "The Thermals", NameSort = "Thermals, The" });
			_agentRepository.Create(new Agent { Name = "The Three O'Clock", NameSort = "Three O'Clock, The" });
			_agentRepository.Create(new Agent { Name = "The Time", NameSort = "Time, The" });
			_agentRepository.Create(new Agent { Name = "The Toasters", NameSort = "Toasters, The" });
			_agentRepository.Create(new Agent { Name = "The Twinkle Brothers", NameSort = "Twinkle Brothers, The" });
			_agentRepository.Create(new Agent { Name = "The Untouchables", NameSort = "Untouchables, The" });
			_agentRepository.Create(new Agent { Name = "The Upsetters", NameSort = "Upsetters, The" });
			_agentRepository.Create(new Agent { Name = "The Velvelettes", NameSort = "Velvelettes, The" });
			_agentRepository.Create(new Agent { Name = "The Velvet Underground", NameSort = "Velvet Underground, The" });
			_agentRepository.Create(new Agent { Name = "The Wailers", NameSort = "Wailers, The" });
			_agentRepository.Create(new Agent { Name = "The Wallets", NameSort = "Wallets, The" });
			_agentRepository.Create(new Agent { Name = "The Who", NameSort = "Who, The" });
			_agentRepository.Create(new Agent { Name = "The Wilders", NameSort = "The Wilders" });
			_agentRepository.Create(new Agent { Name = "The Yardbirds", NameSort = "The Yardbirds" });
			_agentRepository.Create(new Agent { Name = "Thelonious Monk", NameSort = "Monk, Thelonious" });
			_agentRepository.Create(new Agent { Name = "Thinking Fellers Union Local 282", NameSort = "Thinking Fellers Union Local 282" });
			_agentRepository.Create(new Agent { Name = "This Mortal Coil", NameSort = "This Mortal Coil" });
			_agentRepository.Create(new Agent { Name = "Thom Monahan", NameSort = "Monahan, Thom" });
			_agentRepository.Create(new Agent { Name = "Thomas Brenneck", NameSort = "Brenneck, Thomas" });
			_agentRepository.Create(new Agent { Name = "Throwing Muses", NameSort = "Throwing Muses" });
			_agentRepository.Create(new Agent { Name = "Tim Buckley", NameSort = "Buckley, Tim" });
			_agentRepository.Create(new Agent { Name = "Tim Kramer", NameSort = "Kramer, Tim" });
			_agentRepository.Create(new Agent { Name = "Tim Mulligan", NameSort = "Mulligan, Tim" });
			_agentRepository.Create(new Agent { Name = "Times New Viking", NameSort = "Times New Viking" });
			_agentRepository.Create(new Agent { Name = "Tion", NameSort = "Tion" });
			_agentRepository.Create(new Agent { Name = "Toby Scott", NameSort = "Scott, Toby" });
			_agentRepository.Create(new Agent { Name = "Todd Dunnigan", NameSort = "Dunnigan, Todd" });
			_agentRepository.Create(new Agent { Name = "Todd Rundgren", NameSort = "Rundgren, Todd" });
			_agentRepository.Create(new Agent { Name = "Tom Allom", NameSort = "Allom, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Dowd ", NameSort = "Dowd, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Dowd", NameSort = "Dowd, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Flye", NameSort = "Flye, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Hamilton", NameSort = "Hamilton, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Waits", NameSort = "Waits, Tom" });
			_agentRepository.Create(new Agent { Name = "Tom Wilson", NameSort = "Wilson, Tom" });
			_agentRepository.Create(new Agent { Name = "Tommy Garrett", NameSort = "Garrett, Tommy" });
			_agentRepository.Create(new Agent { Name = "Tommy James & The Shondells", NameSort = "James & The Shondells, Tommy" });
			_agentRepository.Create(new Agent { Name = "Tommy McCook", NameSort = "McCook, Tommy" });
			_agentRepository.Create(new Agent { Name = "Tony Allom", NameSort = "Allom, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony Bongiovi", NameSort = "Bongiovi, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony May", NameSort = "May, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony Platt", NameSort = "Platt, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony Rice", NameSort = "Rice, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony Silvestre", NameSort = "Silvestre, Tony" });
			_agentRepository.Create(new Agent { Name = "Tony Visconti", NameSort = "Visconti, Tony" });
			_agentRepository.Create(new Agent { Name = "Toots and the Maytals", NameSort = "Toots and the Maytals" });
			_agentRepository.Create(new Agent { Name = "Tortoise", NameSort = "Tortoise" });
			_agentRepository.Create(new Agent { Name = "Totimoshi", NameSort = "Totimoshi" });
			_agentRepository.Create(new Agent { Name = "Tracy Chapman", NameSort = "Chapman, Tracy" });
			_agentRepository.Create(new Agent { Name = "Traffic", NameSort = "Traffic" });
			_agentRepository.Create(new Agent { Name = "Trans Am", NameSort = "Trans Am" });
			_agentRepository.Create(new Agent { Name = "Treepeople", NameSort = "Treepeople" });
			_agentRepository.Create(new Agent { Name = "Triston Palma", NameSort = "Palma, Triston" });
			_agentRepository.Create(new Agent { Name = "Trouble Funk", NameSort = "Trouble Funk" });
			_agentRepository.Create(new Agent { Name = "Trumans Water", NameSort = "Trumans Water" });
			_agentRepository.Create(new Agent { Name = "Tshala Muana", NameSort = "Muana, Tshala" });
			_agentRepository.Create(new Agent { Name = "Tweak Bird", NameSort = "Tweak Bird" });
			_agentRepository.Create(new Agent { Name = "Tyrone Davis", NameSort = "Davis, Tyrone" });
			_agentRepository.Create(new Agent { Name = "U Roy", NameSort = "U Roy" });
			_agentRepository.Create(new Agent { Name = "U2", NameSort = "U2" });
			_agentRepository.Create(new Agent { Name = "UB40", NameSort = "UB40" });
			_agentRepository.Create(new Agent { Name = "Unwound", NameSort = "Unwound" });
			_agentRepository.Create(new Agent { Name = "Upsetters", NameSort = "Upsetters" });
			_agentRepository.Create(new Agent { Name = "Uriah Heep", NameSort = "Uriah Heep" });
			_agentRepository.Create(new Agent { Name = "U-Roy", NameSort = "U-Roy" });
			_agentRepository.Create(new Agent { Name = "Val Garay", NameSort = "Garay, Val" });
			_agentRepository.Create(new Agent { Name = "Val Valentin", NameSort = "Valentin, Val" });
			_agentRepository.Create(new Agent { Name = "Vampire Weekend", NameSort = "Vampire Weekend" });
			_agentRepository.Create(new Agent { Name = "Van Der Graaf Generator", NameSort = "Van Der Graaf Generator" });
			_agentRepository.Create(new Agent { Name = "Van Der Graaf", NameSort = "Van Der Graaf" });
			_agentRepository.Create(new Agent { Name = "Van Morrison", NameSort = "Morrison, Van" });
			_agentRepository.Create(new Agent { Name = "Vance Powell", NameSort = "Powell, Vance" });
			_agentRepository.Create(new Agent { Name = "Vanda & Young", NameSort = "Vanda & Young" });
			_agentRepository.Create(new Agent { Name = "Various Artists", NameSort = "Various Artists" });
			_agentRepository.Create(new Agent { Name = "Verckys", NameSort = "Verckys" });
			_agentRepository.Create(new Agent { Name = "Veronica Falls", NameSort = "Veronica Falls" });
			_agentRepository.Create(new Agent { Name = "Vetiver", NameSort = "Vetiver" });
			_agentRepository.Create(new Agent { Name = "Vic Chirumbolo", NameSort = "Chirumbolo, Vic" });
			_agentRepository.Create(new Agent { Name = "Victor Jara", NameSort = "Jara, Victor" });
			_agentRepository.Create(new Agent { Name = "Victor Uwaifo", NameSort = "Uwaifo, Victor" });
			_agentRepository.Create(new Agent { Name = "Vince Traina", NameSort = "Traina, Vince" });
			_agentRepository.Create(new Agent { Name = "Vivian Jackson (Yabby You)", NameSort = "Jackson (Yabby You), Vivian" });
			_agentRepository.Create(new Agent { Name = "Volcano Suns", NameSort = "Volcano Suns" });
			_agentRepository.Create(new Agent { Name = "W. Barry Wilson", NameSort = "Wilson, W. Barry" });
			_agentRepository.Create(new Agent { Name = "W. Mattews", NameSort = "Mattews, W. " });
			_agentRepository.Create(new Agent { Name = "Wailing Souls", NameSort = "Wailing Souls" });
			_agentRepository.Create(new Agent { Name = "Wally Buck", NameSort = "Buck, Wally" });
			_agentRepository.Create(new Agent { Name = "Wally Heider", NameSort = "Heider, Wally" });
			_agentRepository.Create(new Agent { Name = "War", NameSort = "War" });
			_agentRepository.Create(new Agent { Name = "Washington Phillips", NameSort = "Phillips, Washington" });
			_agentRepository.Create(new Agent { Name = "Waylon Jennings", NameSort = "Jennings, Waylon" });
			_agentRepository.Create(new Agent { Name = "Wayne Henderson", NameSort = "Henderson, Wayne" });
			_agentRepository.Create(new Agent { Name = "Wayne Horvitz (The President)", NameSort = "Horvitz (The President), Wayne", FileUnder = "Wayne Horvitz" });
			_agentRepository.Create(new Agent { Name = "Wayne Horvitz", NameSort = "Horvitz, Wayne" });
			_agentRepository.Create(new Agent { Name = "Ween", NameSort = "Ween" });
			_agentRepository.Create(new Agent { Name = "Welton Irie", NameSort = "Irie, Welton" });
			_agentRepository.Create(new Agent { Name = "Wes Montgomery", NameSort = "Montgomery, Wes" });
			_agentRepository.Create(new Agent { Name = "West Nkosi", NameSort = "Nkosi, West" });
			_agentRepository.Create(new Agent { Name = "White Mice", NameSort = "White Mice" });
			_agentRepository.Create(new Agent { Name = "Wiggy", NameSort = "Wiggy" });
			_agentRepository.Create(new Agent { Name = "William C. Brown III", NameSort = "Brown III, William C." });
			_agentRepository.Create(new Agent { Name = "William Collins", NameSort = "Collins, William" });
			_agentRepository.Create(new Agent { Name = "Willie Henderson", NameSort = "Henderson, Willie" });
			_agentRepository.Create(new Agent { Name = "Willie Nelson", NameSort = "Nelson, Willie" });
			_agentRepository.Create(new Agent { Name = "Wilson Pickett", NameSort = "Pickett, Wilson" });
			_agentRepository.Create(new Agent { Name = "Winston \"Niney\" Holness aka \"The Observer\"", NameSort = "Holness aka \"The Observer\", Winston \"Niney\"" });
			_agentRepository.Create(new Agent { Name = "Winston Francis", NameSort = "Francis, Winston" });
			_agentRepository.Create(new Agent { Name = "Winston Riley", NameSort = "Riley, Winston" });
			_agentRepository.Create(new Agent { Name = "Wire", NameSort = "Wire" });
			_agentRepository.Create(new Agent { Name = "Woody Guthrie", NameSort = "Guthrie, Woody" });
			_agentRepository.Create(new Agent { Name = "World Saxophone Quartet", NameSort = "World Saxophone Quartet" });
			_agentRepository.Create(new Agent { Name = "X", NameSort = "X" });
			_agentRepository.Create(new Agent { Name = "XTC", NameSort = "XTC" });
			_agentRepository.Create(new Agent { Name = "Yamatsuka Eye", NameSort = "Yamatsuka Eye" });
			_agentRepository.Create(new Agent { Name = "Yami Bolo", NameSort = "Yami Bolo" });
			_agentRepository.Create(new Agent { Name = "Yello", NameSort = "Yello" });
			_agentRepository.Create(new Agent { Name = "Yellowman", NameSort = "Yellowman" });
			_agentRepository.Create(new Agent { Name = "Yes", NameSort = "Yes" });
			_agentRepository.Create(new Agent { Name = "Yo La Tengo", NameSort = "Yo La Tengo" });
			_agentRepository.Create(new Agent { Name = "Zaiko Langa Langa", NameSort = "Zaiko Langa Langa" });
			_agentRepository.Create(new Agent { Name = "Zelia Barbosa", NameSort = "Barbosa, Zelia" });
			_agentRepository.Create(new Agent { Name = "ZZ Top", NameSort = "ZZ Top" });

            }

        private void CreateAmgRatings() {
            #region AmgRatings

            var amgRatings = new[] {
                "1",
                "1.5",
                "2",
                "2.5",
                "3",
                "3.5",
                "4",
                "4.5",
                "5"
            };

            #endregion

            foreach (var amgRating in amgRatings) {
                _amgRatingRepository.Create(new AmgRating {Name = amgRating});
            }
        }

        private void CreateCountries() {
            #region Countries

            var countries = new[] {
                "Afghanistan",
                "Åland Islands",
                "Albania",
                "Algeria",
                "American Samoa",
                "Andorra",
                "Angola",
                "Anguilla",
                "Antarctica",
                "Antigua and Barbuda",
                "Argentina",
                "Armenia",
                "Aruba",
                "Australia",
                "Austria",
                "Azerbaijan",
                "Bahamas",
                "Bahrain",
                "Bangladesh",
                "Barbados",
                "Belarus",
                "Belgium",
                "Belize",
                "Benin",
                "Bermuda",
                "Bhutan",
                "Bolivia (Plurinational State of)",
                "Bonaire, Sint Eustatius and Saba",
                "Bosnia and Herzegovina",
                "Botswana",
                "Bouvet Island",
                "Brazil",
                "British Indian Ocean Territory",
                "Brunei Darussalam",
                "Bulgaria",
                "Burkina Faso",
                "Burundi",
                "Cambodia",
                "Cameroon",
                "Canada",
                "Cabo Verde",
                "Cayman Islands",
                "Central African Republic",
                "Chad",
                "Chile",
                "China",
                "Christmas Island",
                "Cocos (Keeling) Islands",
                "Colombia",
                "Comoros",
                "Congo",
                "Congo (Democratic Republic of the)",
                "Cook Islands",
                "Costa Rica",
                "Côte d'Ivoire",
                "Croatia",
                "Cuba",
                "Curaçao",
                "Cyprus",
                "Czech Republic",
                "Denmark",
                "Djibouti",
                "Dominica",
                "Dominican Republic",
                "Ecuador",
                "Egypt",
                "El Salvador",
                "Equatorial Guinea",
                "Eritrea",
                "Estonia",
                "Ethiopia",
                "Falkland Islands (Malvinas)",
                "Faroe Islands",
                "Fiji",
                "Finland",
                "France",
                "French Guiana",
                "French Polynesia",
                "French Southern Territories",
                "Gabon",
                "Gambia",
                "Georgia",
                "Germany",
                "Ghana",
                "Gibraltar",
                "Greece",
                "Greenland",
                "Grenada",
                "Guadeloupe",
                "Guam",
                "Guatemala",
                "Guernsey",
                "Guinea",
                "Guinea-Bissau",
                "Guyana",
                "Haiti",
                "Heard Island and McDonald Islands",
                "Holy See",
                "Honduras",
                "Hong Kong",
                "Hungary",
                "Iceland",
                "India",
                "Indonesia",
                "Iran (Islamic Republic of)",
                "Iraq",
                "Ireland",
                "Isle of Man",
                "Israel",
                "Italy",
                "Jamaica",
                "Japan",
                "Jersey",
                "Jordan",
                "Kazakhstan",
                "Kenya",
                "Kiribati",
                "Korea (Democratic People's Republic of)",
                "Korea (Republic of)",
                "Kuwait",
                "Kyrgyzstan",
                "Lao People's Democratic Republic",
                "Latvia",
                "Lebanon",
                "Lesotho",
                "Liberia",
                "Libya",
                "Liechtenstein",
                "Lithuania",
                "Luxembourg",
                "Macao",
                "Macedonia (the former Yugoslav Republic of)",
                "Madagascar",
                "Malawi",
                "Malaysia",
                "Maldives",
                "Mali",
                "Malta",
                "Marshall Islands",
                "Martinique",
                "Mauritania",
                "Mauritius",
                "Mayotte",
                "Mexico",
                "Micronesia (Federated States of)",
                "Moldova (Republic of)",
                "Monaco",
                "Mongolia",
                "Montenegro",
                "Montserrat",
                "Morocco",
                "Mozambique",
                "Myanmar",
                "Namibia",
                "Nauru",
                "Nepal",
                "Netherlands",
                "New Caledonia",
                "New Zealand",
                "Nicaragua",
                "Niger",
                "Nigeria",
                "Niue",
                "Norfolk Island",
                "Northern Mariana Islands",
                "Norway",
                "Oman",
                "Pakistan",
                "Palau",
                "Palestine, State of",
                "Panama",
                "Papua New Guinea",
                "Paraguay",
                "Peru",
                "Philippines",
                "Pitcairn",
                "Poland",
                "Portugal",
                "Puerto Rico",
                "Qatar",
                "Réunion",
                "Romania",
                "Russian Federation",
                "Rwanda",
                "Saint Barthélemy",
                "Saint Helena, Ascension and Tristan da Cunha",
                "Saint Kitts and Nevis",
                "Saint Lucia",
                "Saint Martin (French part)",
                "Saint Pierre and Miquelon",
                "Saint Vincent and the Grenadines",
                "Samoa",
                "San Marino",
                "Sao Tome and Principe",
                "Saudi Arabia",
                "Senegal",
                "Serbia",
                "Seychelles",
                "Sierra Leone",
                "Singapore",
                "Sint Maarten (Dutch part)",
                "Slovakia",
                "Slovenia",
                "Solomon Islands",
                "Somalia",
                "South Africa",
                "South Georgia and the South Sandwich Islands",
                "South Sudan",
                "Spain",
                "Sri Lanka",
                "Sudan",
                "Suriname",
                "Svalbard and Jan Mayen",
                "Swaziland",
                "Sweden",
                "Switzerland",
                "Syrian Arab Republic",
                "Taiwan, Province of China",
                "Tajikistan",
                "Tanzania, United Republic of",
                "Thailand",
                "Timor-Leste",
                "Togo",
                "Tokelau",
                "Tonga",
                "Trinidad and Tobago",
                "Tunisia",
                "Turkey",
                "Turkmenistan",
                "Turks and Caicos Islands",
                "Tuvalu",
                "Uganda",
                "Ukraine",
                "United Arab Emirates",
                "United Kingdom of Great Britain and Northern Ireland",
                "United States of America",
                "United States Minor Outlying Islands",
                "Uruguay",
                "Uzbekistan",
                "Vanuatu",
                "Venezuela (Bolivarian Republic of)",
                "Viet Nam",
                "Virgin Islands (British)",
                "Virgin Islands (U.S.)",
                "Wallis and Futuna",
                "Western Sahara",
                "Yemen",
                "Zambia",
                "Zimbabwe"
            };

            #endregion

            foreach (var country in countries) {
                _countryRepository.Create(new Country {Name = country});
            }
        }

        private void CreateFormats() {
            #region Formats

            var formats = new[] {
                "Vinyl - 7",
                "Vinyl - 10",
                "Vinyl - 12"
            };

            #endregion

            foreach (var format in formats) {
                _formatRepository.Create(new Format {Name = format});
            }
        }

        private void CreateGenres() {
            #region Genres

            var genres = new[] {
                "Acid Rock",
                "Africa",
                "African Jazz",
                "Afrobeat",
                "Afropop",
                "Algeria",
                "Alternative",
                "Ambient",
                "Americana",
                "Angola",
                "Appalachia",
                "Arabic",
                "Armenia",
                "Art Rock",
                "Asian Pop",
                "Athens, GA Scene",
                "Avant-Garde",
                "Beer Drinking Songs",
                "Belly Dance",
                "Bluegrass",
                "Blues",
                "Bossa Nova",
                "Brazil",
                "Brazilian Pop",
                "Brit Pop",
                "British Folk",
                "British Folk-Rock",
                "Brit-Pop",
                "Cajun",
                "Cambodia",
                "Cameroon",
                "Camp Songs",
                "Caribbean",
                "Celtic",
                "Childrens",
                "Chile",
                "Classic Rock",
                "Classical",
                "Classical Guitar",
                "Comedy",
                "Congolese",
                "Country",
                "Creole",
                "Dancehall",
                "DC",
                "Disco",
                "DJ",
                "Dub",
                "Dub Poetry",
                "Early Rock",
                "Easy Listening",
                "Electronic",
                "Europe",
                "Exotica",
                "Experimental",
                "Flamenco",
                "Folk",
                "Folk Revival",
                "Folk-Rock",
                "France",
                "Free Jazz",
                "French Pop",
                "Funk",
                "Fusion",
                "Garage Rock",
                "Germany",
                "Glam Rock",
                "Go Go",
                "Gospel",
                "Grunge",
                "Guinea",
                "Guitar",
                "Gypsy",
                "Hammond B3",
                "Hard Rock",
                "Hardcore",
                "Harp",
                "Heavy Metal",
                "Highlife",
                "Hip-Hop",
                "Holidays",
                "Improv",
                "Indian Classical",
                "Indie-Rock",
                "Instructional",
                "Instrumental",
                "Instrumental Reggae",
                "Instrumental Rock",
                "Irish",
                "Italy",
                "Jamaica",
                "Japan",
                "Jazz",
                "Juju",
                "Krautrock",
                "Latin",
                "Latin Jazz",
                "Lounge",
                "Lovers Rock",
                "Mali",
                "Math Rock",
                "Mbaqanga",
                "Memphis Soul",
                "Metal",
                "Middle East",
                "Minimalism",
                "Miscellaneous",
                "Mod",
                "Musical Theater",
                "Nature Sounds",
                "New Age",
                "New Orleans",
                "New Wave",
                "Nigeria",
                "Noise",
                "North Africa",
                "NW Rock",
                "Organ Music",
                "Pipe Organ",
                "Poetry",
                "Political",
                "Pop",
                "Popular Music",
                "Post-Punk",
                "Post-Rock",
                "Pre-War Blues",
                "Prog-Rock",
                "Protest",
                "Psychedelic",
                "Punk",
                "R&B",
                "Ragtime",
                "Rai",
                "Rap",
                "Reggae",
                "Religion",
                "Rock",
                "Rock Opera",
                "Rockabilly",
                "Rocksteady",
                "Roots",
                "Rumba",
                "Samba",
                "Seattle",
                "SF Scene",
                "Singer-Songwriter",
                "Sixties",
                "Ska",
                "Ska Revival",
                "Slowcore",
                "So Bad It's Good",
                "Soukous",
                "Soul",
                "Soul-Jazz",
                "Sound Recording",
                "Soundtracks",
                "South Africa",
                "Spain",
                "Spirituals",
                "Spoken Word",
                "Steel Bands",
                "Stoner Rock",
                "Surf",
                "Swing",
                "Tanzania",
                "Traditional",
                "Traditional Folk",
                "Traditional World",
                "Tropicalia",
                "Turkey",
                "TV",
                "Twee Pop",
                "Vocals",
                "World",
                "Zaire",
                "Zimbabwe",
                "Zydeco"
            };

            #endregion

            foreach (var genre in genres) {
                _genreRepository.Create(new Genre {Name = genre});
            }
        }

        private void CreateLengths() {
            #region Lengths

            var lengths = new[] {
                "EP",
                "EP-2",
                "LP",
                "LP-2",
                "LP-3",
                "LP-4",
                "LP-5",
                "LP-6",
                "Single",
                "Single-2",
                "Single-3",
                "Single-4",
                "Single-5",
                "Single-6",
                "Single-7",
                "Single-8"
            };

            #endregion

            foreach (var length in lengths) {
                _lengthRepository.Create(new Length {Name = length});
            }
        }

        private void CreateRecordLabels() {
            #region RecordLabels

            var recordLabels = new[] {
                "13th Floor",
                "1972",
                "20/20/20",
                "20th Century Fox",
                "20th Century",
                "4 Men With Beards",
                "4AD",
                "5th Element",
                "97 Crew",
                "A&M",
                "ABC",
                "ABC-Paramount",
                "Abduction",
                "Abraham",
                "Actuel",
                "Adelphi",
                "Affinity",
                "Afric Music",
                "African Love Music",
                "African Museum",
                "African Record Centre Stores",
                "African",
                "Ahmek",
                "Alaba",
                "Albion",
                "Alligator",
                "Alternative Tentacles",
                "Amazing Grease",
                "Amphetamine Reptile",
                "Amulet",
                "Amy",
                "Anchor and Hope",
                "Andrew Hill",
                "Apple",
                "Aquarius",
                "Archive",
                "Ardent",
                "Arena Rock",
                "Arhoolie",
                "Arista",
                "Armory Arms",
                "Arts & Crafts",
                "Asylum",
                "At The Dojo",
                "Atco",
                "Atlantic",
                "Attack",
                "Audio Fidelity",
                "Audiophile",
                "Audiotex",
                "Baby Father",
                "Bang",
                "Bar/None",
                "Barbes Records",
                "Barclay",
                "Basement",
                "Bayou Records",
                "BBC",
                "Bearsville",
                "Belltown",
                "Big Cat",
                "Big Time",
                "Black Art",
                "Black Lion",
                "Black Saint",
                "Black Solidarity",
                "Black Top Records",
                "Black Top",
                "Blank",
                "Blast First",
                "Blood & Fire",
                "Blood and Fire",
                "Blue Beat",
                "Blue Note",
                "BMG",
                "Bold Records",
                "Bold",
                "Breakfast Music",
                "Bronze",
                "Brunswick",
                "Buddah",
                "Burning Sounds",
                "BYG",
                "C/Z Records",
                "Caedmon",
                "Calla",
                "Canadian Talent Library Trust",
                "Candid",
                "Capitol",
                "Capricorn",
                "Captured Tracks",
                "Caroline",
                "Carosello",
                "Casablanca",
                "Cat",
                "Cavity Search",
                "CBS",
                "CD Presents",
                "Celluloid",
                "Channel One",
                "Charisma",
                "Charly",
                "Charmax",
                "Checkmate",
                "Chelsea",
                "Chemical Imbalance",
                "Cherry Red",
                "Cherrytree",
                "Chess",
                "Chiaroscuro",
                "Chisa",
                "Choc Choc Choc",
                "Chocodog",
                "Chrysalis",
                "Chunk Records",
                "City Slang",
                "Clef",
                "Clocktower",
                "Cobblestone",
                "Collectables",
                "Collectibles",
                "Columbia",
                "Compleat",
                "Cook Records",
                "Cook",
                "Cotillion",
                "Cottilion",
                "Coxsone",
                "Crescendo",
                "Crooked Beat",
                "CTI",
                "Cuneiform",
                "Curtom",
                "Dakar",
                "Daptone",
                "DB  Recs",
                "DB Recs",
                "DB",
                "Death Row",
                "Decca",
                "Def Jam",
                "Delmark",
                "Demon",
                "Dischord",
                "DKM",
                "Dog Gone",
                "Domino",
                "Dossier",
                "Drag City",
                "DRG",
                "Duophonic UHF Disks",
                "E.G. Records",
                "E.G.",
                "Earmark",
                "Earthworks",
                "Earworm",
                "ECM",
                "Ecstatic Peace",
                "Edipop",
                "EG",
                "El Rey",
                "Electraphonic",
                "Electro Motive",
                "Elektra",
                "Embryo",
                "EMI Electrola",
                "EMI",
                "EMIDisc",
                "EMI-Odeon",
                "Empty Records",
                "Empty",
                "Emus",
                "Enemy",
                "Enigma",
                "Enja",
                "Epic ",
                "Epic",
                "Eric",
                "Everest",
                "Experience Hendrix",
                "Eye Q",
                "Face The Music",
                "Factory",
                "Fader",
                "Fantasy",
                "Fat Cat",
                "Fat Eyes",
                "Fat Possum",
                "Fatbeats",
                "Fatman",
                "F-Beat",
                "Federal",
                "Fillmore",
                "Finders Keepers",
                "Fine Art",
                "Fire",
                "Fireball",
                "Flashback",
                "Flying Dutchman",
                "Flying Fish",
                "Folk-Legacy",
                "Folkways Records",
                "Folkways",
                "Free Dirt ",
                "Free Dirt",
                "Freedom Sounds",
                "Frontier",
                "FU2U2",
                "Fundamental",
                "Galaxy",
                "Garcia",
                "Geffen",
                "Genidia",
                "Get Back",
                "Get Hip",
                "Giant Step",
                "Glass Fish",
                "Global Rhythm Group",
                "GNP Crescendo",
                "GNP Crescendo, Los Angeles, CA",
                "GNP",
                "Go Feet",
                "Go! Discs",
                "Go!",
                "Go-Feet",
                "Golden",
                "Goldshower",
                "Goofin",
                "Graduate",
                "Gramavision",
                "Grateful Dead",
                "Greensleeves",
                "Groenland",
                "Gronland",
                "Groove Merchant",
                "Grunt",
                "Gull",
                "GWR",
                "Hall Of Fame",
                "Hallmark",
                "Hannibal",
                "Harry J's",
                "Harvest",
                "Heartbeat",
                "High Fidelity",
                "Hinter",
                "Hit Bound",
                "Hitbound",
                "Hollywood",
                "Home Recorded Cassette Culture",
                "Home Tapes",
                "Homestead",
                "Honest Jons",
                "Honest Jon's",
                "Huh-Bag Records",
                "I.R.S.",
                "Immediate",
                "Impact!",
                "Impulse!",
                "Impulse",
                "Independent Project",
                "Inner City",
                "International Feel",
                "Interscope",
                "Ipecac",
                "IRS",
                "Island Def Jam",
                "Island",
                "Izniz",
                "J Street",
                "Jackpot",
                "JAD",
                "Jagjaguwar",
                "Jaguar",
                "Jah Thomas",
                "Jamaican Recordings",
                "Jammy's",
                "Japan Overseas",
                "Jazzman",
                "JDC",
                "Jewel",
                "Jive Afrika",
                "Jive",
                "Joe Gibbs",
                "Josie",
                "Joyful Noise",
                "Justice League",
                "Justice",
                "K ",
                "K",
                "Kaleidoscope",
                "Kapp",
                "Kent",
                "Key Production",
                "Keyhole",
                "Keyman",
                "Kill Rock Stars",
                "Kindred Spirits",
                "King",
                "Knitting Factory",
                "Konkurrent",
                "Korova",
                "Kranky",
                "Krazy Kat",
                "Legacy",
                "Les Disques Motors",
                "Lexicon",
                "Liberty",
                "Life",
                "Light In The Attic",
                "Lillith",
                "Limelight",
                "Line",
                "Lineatre",
                "Live & Learn",
                "Locust",
                "London",
                "Loosegroove",
                "Lost-Nite",
                "Luaka Bop",
                "Lucasfilm",
                "Mafia",
                "Magnatone",
                "Maison de Soul",
                "Majora",
                "Makossa",
                "Mammoth",
                "Manglo",
                "Mango",
                "Matador",
                "MCA",
                "Mega",
                "Melodisc",
                "Mercury",
                "Merge",
                "Metalanguage",
                "MGM",
                "Michigan",
                "MID",
                "Midi-France",
                "Mississippi",
                "Monitor",
                "Monument",
                "Moon",
                "Morwell",
                "Motown",
                "Mr. Bongo",
                "Muse",
                "Music For Pleasure",
                "Music Works",
                "Musical Heritage",
                "Musicor",
                "Musidisc Brazil",
                "Musidisc",
                "Myrrh",
                "New Alliance",
                "New Jazz",
                "Next Ambiancec",
                "Nighthawk",
                "Ninth Street",
                "Nonesuch",
                "Northern Spy",
                "Norton",
                "Obey",
                "Observer",
                "Ocora",
                "Old Town",
                "Om",
                "Opal",
                "Orchid",
                "Oslo Grammofon",
                "OSOM",
                "Paisley Park",
                "Palace",
                "Palilalia",
                "Pantomine",
                "Paramount",
                "Paredon",
                "Passport",
                "Pathe",
                "Peacock",
                "People",
                "Perception",
                "Philips",
                "Philles",
                "Phonogram",
                "PI Recordings",
                "Pickwick",
                "PIP",
                "Pitch-A-Tent",
                "Placebo",
                "Plain",
                "Plowboy",
                "Polydor",
                "Polygram",
                "Polyvinyl",
                "Poon Village",
                "Powderworks",
                "Premier",
                "Pressure Sounds",
                "Prestige",
                "Prince Buster ",
                "Prince Buster Record Shack, Kingston",
                "Prince Buster",
                "Prince Jazzbo",
                "Productions Souk",
                "Profile",
                "Prophet",
                "Quarterstick",
                "Qwest",
                "R&B",
                "Radiopaque",
                "Ralph",
                "Randy's",
                "Rare Bird",
                "RCA",
                "Real World",
                "Real",
                "Rebel Records",
                "Rebel",
                "Recommended",
                "Red Label",
                "Red Lightnin'",
                "Red Stripe",
                "Reflex",
                "Relix",
                "Reprise  ",
                "Reprise",
                "Request",
                "Rhino",
                "Rift",
                "Riverside",
                "Riviera Global",
                "Rocky 1",
                "Rogers All Stars",
                "Rolling Stones",
                "Rough Justice",
                "Rough Trade",
                "Roulette",
                "Round ",
                "Round Records",
                "Round",
                "Rounder",
                "RSO",
                "Ryko Analogue",
                "Ryko",
                "Sackville",
                "Sanctuary",
                "Saturnine",
                "Savoya",
                "Schnitzel",
                "Scooch Pooch",
                "Secretly Canadian",
                "See For Miles",
                "Shanachie",
                "Shelter",
                "Shepherd",
                "Shimmy-Disc",
                "Shortstack",
                "Silver Spotlight",
                "Sire",
                "Sky",
                "Slash",
                "Slo Poke",
                "Slumberland",
                "Smash",
                "Smithsonian Folkways",
                "Sockets Records",
                "Sockets",
                "Sonic Sounds",
                "Sonodisc",
                "Sony",
                "Soul Jazz",
                "Soul Static",
                "Sound Aspects",
                "Sound Triangle",
                "Southern Lord",
                "Spacebomb",
                "SST",
                "Star Musique",
                "Stax ",
                "Stax",
                "Sterns",
                "Stiff",
                "Stones Throw",
                "Strange Fruit",
                "Street Wise",
                "Striker Lee",
                "Studio 1",
                "Studio One",
                "Sub Pop",
                "Sugar Hill Records, Durham, NC",
                "Sugarhill",
                "Suicide Squeeze",
                "Sultan",
                "Summus",
                "Sun",
                "Sundazed",
                "Swan Song",
                "Sweet Nothing",
                "Sympathy for the Record Industry",
                "Syntonic Research",
                "Taang",
                "Takoma",
                "Tamla",
                "TBD",
                "Team Lizabean",
                "Techniques",
                "Temporary Residence",
                "Testament",
                "Theatre For Your Mother",
                "Third Gear",
                "Third Man",
                "Third World",
                "Thompson Sound",
                "Thrill Jockey",
                "Too Pure",
                "Topic",
                "Touch and Go",
                "Tough Love",
                "Trade Root",
                "Tradition",
                "Treasure Isle",
                "Treble Kicker",
                "Tribe",
                "Trip Jazz",
                "Triple X",
                "Trojan",
                "Tuff Gong",
                "Twin Tone",
                "Twinkle",
                "Ubiquity",
                "United Artists",
                "Universal Republic",
                "Universal",
                "UP Records",
                "Up Roar",
                "Up",
                "Upfront",
                "Upsetter",
                "Utopia",
                "Vanguard",
                "Varese",
                "Vee Jay",
                "Vee-Jay",
                "Vernon Yard",
                "Versatile",
                "Vertigo",
                "Verve Folkways",
                "Verve Forecast",
                "Verve",
                "Victor",
                "Virgin",
                "Volcano",
                "Volcom",
                "Volt",
                "VP",
                "Wantage USA",
                "Warner Bros.",
                "Warner Brothers",
                "Warner Special Projects",
                "Warner",
                "Warner-Chappell",
                "Waterhouse",
                "WEA",
                "Well Charge",
                "Westminster",
                "What Music",
                "WhatMusic",
                "Why-Fi",
                "World Music Network",
                "World Pacific",
                "Worthy",
                "XL",
                "Yabby You",
                "Zappa"
            };

            #endregion

            foreach (var recordLabel in recordLabels) {
                _recordLabelRepository.Create(new RecordLabel {Name = recordLabel});
            }
        }

        private void CreateRoles() {
            #region Roles

            var roles = new[] {
                "Arranger",
                "Artist",
                "Assistant Engineer",
                "Associate Producer",
                "Audio Restoration",
                "Author",
                "Cinematographer",
                "Collector",
                "Compiler",
                "Composer",
                "Contributor",
                "Co-Producer",
                "Cover Artwork",
                "Designer",
                "Director",
                "Editor",
                "Engineer",
                "Field Worker",
                "Interviewer",
                "Liner Notes",
                "Liner Notes Editor",
                "Lyricist",
                "Mastering Engineer",
                "Mixer",
                "Music Consultant",
                "Narrator",
                "Partner Organization",
                "Performer",
                "Photographer",
                "Photographer (Cover)",
                "Producer",
                "Recorder",
                "Translator",
                "Videographer",
                "Visual Artist",
                "Writer",
                "Writer (Introduction)"
            };

            #endregion

            foreach (var role in roles) {
                _roleRepository.Create(new Role {Name = role});
            }
        }
    }
}