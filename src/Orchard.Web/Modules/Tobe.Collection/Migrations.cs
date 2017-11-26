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
                .WithPart("CategoriesContainerPart")
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

        public int UpdateFrom2()
        {

            ContentDefinitionManager.AlterTypeDefinition("Album", type => type
                .WithPart("CategoriesContainerPart")
                );
            // Categories
            SchemaBuilder.CreateTable("Category", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContainerId")
                .Column<int>("Genre_Id")
                );

            // ArtistsContainerPart
            ContentDefinitionManager.AlterPartDefinition("CategoriesContainerPart", part => part
                .Attachable()
                .WithDescription("Enables your content types to store categories."));

            return 3;
        }
        public int UpdateFrom3()
        {

            ContentDefinitionManager.AlterTypeDefinition("Album", type => type
                .WithPart("CategoriesContainerPart")
                .Indexed("Search")
                );

            return 4;
        }
        public int UpdateFrom4()
        {

            ContentDefinitionManager.AlterTypeDefinition("Album", type => type
                .WithPart("AlbumPart")
                .Indexed("Search")
                );

            return 5;
        }

    }
}