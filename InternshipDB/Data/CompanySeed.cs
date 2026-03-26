using InternshipDB.Models;
using InternshipDB.Helpers;
using Microsoft.EntityFrameworkCore;

namespace InternshipDB.Data
{
    public static class CompanySeed
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            EnsureInformationColumn(db);

            if (db.Companies.Any())
            {
                NormalizeExistingCompanies(db);
                EnsureRequiredCompanies(db);
                return;
            }

            var companies = new[]
            {
                new Company { CompanyName = "3Plex Group", Sector = "Aviation", PersonInCharge = "Paul Fenech", Email = "hr@3plexgroup.com", ContactNumber = "99036205HA", InternshipPeriod = "08:00 - 17:00 (1hr break)", Information="Maintenance Trainee. Address: Centris Business Gateway, Level 3, Triq is-Salib tal-Imriehel, Zone 3 CBD, Birkirkara - Work Location: Aviation Cosmetics Malta, Safi" },
                new Company { CompanyName = "Allelon Hospitality", Sector = "Hospitality", PersonInCharge = "Aldo Allelon", Email = "aldo@allelon.com.mt", ContactNumber = "+356 2123 9035", InternshipPeriod = "", Information="33 Battery Street, Valletta VLT 1222, Malta. Reg: C 64420" },
                new Company { CompanyName = "Abertax Kemtronics Limited", Sector = "Electronics", PersonInCharge = "Christine Deguara", Email = "christine.deguara@abertax.com", ContactNumber = "+356 20168200", Website = "www.abertax.com", InternshipPeriod = "", Information="F9 Bologna Centre, Mosta Technopark, Mosta MST 3000, Malta. Reg: C 58284" },
                new Company { CompanyName = "Alfie's Hair & Beauty Salon", Sector = "Hair & Beauty", PersonInCharge = "Franchesca Rizzo", Email = "alfieshairandbeauty@gmail.com", ContactNumber = "(00356)2133 4362", InternshipPeriod = "", Information="Alfie's Hair Beauty & Nails Salon Triq tas-Sliema , Gzira, Malta GZR 1635. Dress code: uniforms or dressed in black casual/smart, well groomed hair, nails and makeup. 5 days a week/ 7 to 8 hours a day. VAT: C 59198" },
                new Company { CompanyName = "Alpha Bravo Marine", Sector = "Marine", PersonInCharge = "Antoine Barbara", Email = "info@alphabravo.mt", ContactNumber = "+356 99409836", InternshipPeriod = "", Information="Il-Fgura, Malta" },
                new Company { CompanyName = "Atelier del Restauro ltd", Sector = "Restoration", PersonInCharge = "", Email = "info@atelierdelrestauro.com", ContactNumber = "+356 7953 4766", InternshipPeriod = "", Information="B3 Midland Microenterprise Park Burmarrad Road, Naxxar, Malta" },
                new Company { CompanyName = "Malta National Aquarium", Sector = "Tourism", PersonInCharge = "Luke Muscat", Email = "lmuscat@aquarium.com.mt", ContactNumber = "+356 7996 5648", InternshipPeriod = "", Information="Triq it-Trunciera, San Pawl il-Bahar, SPB1500. Reg: C52988, Hours: 40, Dress code: Casual" },
                new Company { CompanyName = "AVA Tech (Electrical Installation)", Sector = "Electrical", PersonInCharge = "Giovanni Galea", Email = "giovanni.galea@gmail.com", ContactNumber = "00356 9994 8980", InternshipPeriod = "", Information="34 F.Geneste, Birkirkara BKR 1683, Malta" },
                new Company { CompanyName = "Armstrong Woodworks Limited", Sector = "Woodworks", PersonInCharge = "Gordon Farrugia", Email = "", ContactNumber = "21447377", InternshipPeriod = "", Information="MST 002B, Valletta Road, Mosta Techno Park, Mosta MST 3000, Malta" },
                new Company { CompanyName = "Bortex Group", Sector = "Clothing", PersonInCharge = "Joshua Zammit", Email = "joshuazammit@bortexgroup.com", ContactNumber = "(00356) 2125 2184", InternshipPeriod = "", Information="A11, Marsa Industrial Estate, Marsa. Reg: C 4863" },
                new Company { CompanyName = "Borza", Sector = "Finance/Other", PersonInCharge = "Alfred Sammut", Email = "asammut@borzamalta.com.mt", ContactNumber = "", InternshipPeriod = "", Information="The Garrison Chapel Castille Valleeta" },
                new Company { CompanyName = "BORN", Sector = "IT", PersonInCharge = "Kurt Abela", Email = "kurt@born.mt", ContactNumber = "+35679201604", InternshipPeriod = "", Information="Born Digital Studio, Triq G. Abela, Haz-Zebbug, Malta, ZBG1722. Position: Python/Javascript developers. Reg: C61408. 9am-5pm. Casual." },
                new Company { CompanyName = "Calum Auto Repairs", Sector = "Automotive", PersonInCharge = "Calum Muscat", Email = "calum.muscat@gmail.com", ContactNumber = "00356 9966 2667", InternshipPeriod = "", Information="Garage 4, Patri Wistin Magro Street, Burmarrad, San Pawl il-Ba?ar SPB 6027, Malta" },
                new Company { CompanyName = "CARISMA SPA & WELLNESS INTERNATIONAL LTD", Sector = "Wellness", PersonInCharge = "Svetlana Matviets", Email = "info@carismaspa.com", ContactNumber = "+356 99900995", InternshipPeriod = "", Information="114 Mert, Triq il-Mizura SWIEQI SWQ2064 Malta. Reg: C 51778" },
                new Company { CompanyName = "Construct Furniture Ltd", Sector = "Furniture", PersonInCharge = "Deborah Gaffiero", Email = "gaffiero.d@constructfurniture.com.mt", ContactNumber = "+356 2122 0610", InternshipPeriod = "", Information="40 CONSTRUCT FURNITURE QASAM INDUSTRIJALI TA' HAL LUQA HAL LUQA, LQA 3000, Malta. 40 hours a week (8 am to 4 pm). Comfortable Clothes and safety shoes. Reg: C 11786" },
                new Company { CompanyName = "Carlo Schembri Ltd", Sector = "Design", PersonInCharge = "Carlo Schembri", Email = "", ContactNumber = "+356 7990 0000", InternshipPeriod = "", Information="Burnarrad behind Lydle St. Pauls Bay, Malta. Smart Casual" },
                new Company { CompanyName = "Camilleri Group of Companies", Sector = "Various", PersonInCharge = "Rodianne Galea", Email = "", ContactNumber = "+356 2147 2255", InternshipPeriod = "", Information="13-20, Triq M.Borg Gauci Handaq - Qormi, Malta. 8:00 to 15:30 (Monday to Friday). Smart Casual. Reg: C 27495" },
                new Company { CompanyName = "Coora Pharmacy", Sector = "Pharmacy", PersonInCharge = "Ivan", Email = "ivan@healthdirect.mt", ContactNumber = "+356 2149 2151", InternshipPeriod = "", Information="Valley Road, Birkirkara, BKR 9022" },
                new Company { CompanyName = "Centric Computer Store", Sector = "Retail/IT", PersonInCharge = "Mr. Keiran Sant", Email = "info@centricmt.com", ContactNumber = "+356 77469333", InternshipPeriod = "", Information="Bayview, Triq San Pawl San Pawl il- Bahar. VAT: MT29511516" },
                new Company { CompanyName = "DB Group (db Seabank)", Sector = "Hospitality", PersonInCharge = "CLAUDINE VALASTRO", Email = "cvalastro@dbhotelsresorts.com", ContactNumber = "+356 2289 1822", InternshipPeriod = "", Information="Marfa Road, Mellieha Bay, Mellieha, MLH 9064. Reg: C 93288" },
                new Company { CompanyName = "Delta Sierra Marine", Sector = "Marine", PersonInCharge = "David Scerri", Email = "info@deltasierra.com.mt", ContactNumber = "(00356)99203055", InternshipPeriod = "", Information="WF4H+P46, Triq Bellavista, San ?wann SGN 2801, Malta" },
                new Company { CompanyName = "DBR Group", Sector = "Various", PersonInCharge = "Dion Barbieri", Email = "dion@dbgroupmalta.com", ContactNumber = "(00356)99000480", InternshipPeriod = "", Information="Dawret-II- Qawra, San Pawl il-Bahar, Malta. Vat: Mt23286415" },
                new Company { CompanyName = "Danseller Co. Ltd", Sector = "Administration", PersonInCharge = "Ivan Angeloski", Email = "ivan.angeloski22@gmail.com", ContactNumber = "+356 99120154", InternshipPeriod = "", Information="St. George Business Center St. Julians. 9 am to 5 pm. Smart Casual. Lapton needed." },
                new Company { CompanyName = "DLI CONTRACTOR LTD", Sector = "Engineering", PersonInCharge = "Ms. Christine", Email = "info@dli.com.mt", ContactNumber = "", InternshipPeriod = "", Information="Level 2, D-Hub, Triq Is-Salib Ta' L-Imriehel, Birkirkara CBD 4020. Roster Based. Safety Shoes." },
                new Company { CompanyName = "D'Argens Pharmacy", Sector = "Pharmacy", PersonInCharge = "Max Borg Millo", Email = "maxbm@collinswilliams.com", ContactNumber = "+356 2124 4847", InternshipPeriod = "", Information="Rue d'Argens Street, G?ira, GZR 1362" },
                new Company { CompanyName = "De Rohan Pharmacy", Sector = "Pharmacy", PersonInCharge = "Michael Grima", Email = "info@grimapharma.com", ContactNumber = "+356 7928 4307", InternshipPeriod = "", Information="24, Triq Sant Antnin, Zebbug, ZBG 2235" },
                new Company { CompanyName = "Delta Pharmacy", Sector = "Pharmacy", PersonInCharge = "Francesca Doublet", Email = "deltapharmacy@jvpharma.eu", ContactNumber = "+356 2168 6665", InternshipPeriod = "", Information="Triq Villa Abate, Zabbar, ZBR 2519" },
                new Company { CompanyName = "EasyJet", Sector = "Aviation", PersonInCharge = "Rita Gorsansawood", Email = "rita.gorwoood@srtechnics.com.mt", ContactNumber = "+356 2249 9110", InternshipPeriod = "", Information="VF48+2M, Luqa, Malta" },
                new Company { CompanyName = "Europa Hotel by ES Hotels", Sector = "Hospitality", PersonInCharge = "Michael Coppini", Email = "europa.manager@eshotelsmalta.com", ContactNumber = "+356 2133 4070", InternshipPeriod = "", Information="138 Tower Road, Sliema SLM 1604. 07.10 - 10-10-13.00. Smart Casual" },
                new Company { CompanyName = "EVA at SALTWAVE LTD.", Sector = "Various", PersonInCharge = "Meareid", Email = "info@Saltwave.eu", ContactNumber = "0035699995374", InternshipPeriod = "", Information="KKW 014, Corradino Industrial Park, Paola PLA 3000" },
                new Company { CompanyName = "ERRC HQ", Sector = "Rescue", PersonInCharge = "Matthew Montebello", Email = "matthew.montebello@um.edu.mt", ContactNumber = "+356 2707 2652", InternshipPeriod = "", Information="Triq ta’ Sannat, Xewkija. Gozo. 9 am to 5 pm. Smart Casual." },
                new Company { CompanyName = "Excel Homes", Sector = "Real Estate", PersonInCharge = "Audrey Grech", Email = "info@excel.com.mt", ContactNumber = "+356 7900 4056", InternshipPeriod = "", Information="Triq il-Linja Attard. 35 Hrs. Office wear." },
                new Company { CompanyName = "EcoMarine Malta", Sector = "Marine", PersonInCharge = "Patrizia Patti", Email = "info@ecomarinemalta.com.mt", ContactNumber = "+35677039356", InternshipPeriod = "", Information="Oakfileds 4 Triq Nicolo' Isouard 18 MST1135 Mosta" },
                new Company { CompanyName = "First Step Nursery", Sector = "Education", PersonInCharge = "Melissa Paris", Email = "firststepsmalta@gmail.com", ContactNumber = "+356 2732 1213", InternshipPeriod = "", Information="38, ANTONIO SCHEMBRI STREET, KAPPARA, SAN GWAN" },
                new Company { CompanyName = "ForFun Design Ltd (Decorama)", Sector = "Design", PersonInCharge = "Emma Fenech Cefai", Email = "emma@decorama.com.mt", ContactNumber = "+356 9916 3266", InternshipPeriod = "", Information="Midland Micro Enterprise Park, E3 Road, Naxxar. 35 hours a week." },
                new Company { CompanyName = "Five-Senses Malta", Sector = "Administration", PersonInCharge = "Sladana Nikolic", Email = "info@5sensesmalta.com", ContactNumber = "", InternshipPeriod = "", Information="100 Main Street Zejtun. 9 am to 5 pm, Smart Casual" },
                new Company { CompanyName = "Fire & Security Engineering", Sector = "Engineering", PersonInCharge = "Ms. Synthia", Email = "info@fse.com.mt", ContactNumber = "+356 2122 7746", InternshipPeriod = "", Information="Dawret ?al G?axaq, ?al G?axaq. 9 am to 3 pm" },
                new Company { CompanyName = "Fatima Pharmacy", Sector = "Pharmacy", PersonInCharge = "Hannah", Email = "fatima@jvpharma.eu", ContactNumber = "+356 2148 2856", InternshipPeriod = "", Information="82 Triq il-Ferrovija, Santa Venera" },
                new Company { CompanyName = "Golden Care Ltd.", Sector = "Healthcare", PersonInCharge = "Carmela Mutuc", Email = "carmela@goldencare.com.mt", ContactNumber = "0035699166119", InternshipPeriod = "", Information="TRIQ IL-WIRT NATURALI, BAHAR IC-CAGHAQ, NAXXAR NXR 5232" },
                new Company { CompanyName = "Go Bananas Gift Shop", Sector = "Retail", PersonInCharge = "Mr. Charmaine.", Email = "", ContactNumber = "+35699998496", InternshipPeriod = "", Information="247 Republic St, Valletta. 9 am to 4 pm" },
                new Company { CompanyName = "Grand Parents Malta Foundation", Sector = "NGO", PersonInCharge = "Philip Chircop", Email = "Phchircop@gmail.com", ContactNumber = "00356 9920 7043", InternshipPeriod = "", Information="Triq il-Parata, Santa Venera, SVR1311" },
                new Company { CompanyName = "Heritage Malta", Sector = "Heritage", PersonInCharge = "Marl Duca", Email = "marl.duca@gov.mt", ContactNumber = "+356 22954345", InternshipPeriod = "", Information="35 Dawret Fra Giovanni Bichi, Kalkara KKR 1280" },
                new Company { CompanyName = "Gamebreaker", Sector = "Retail", PersonInCharge = "Kirsten", Email = "", ContactNumber = "27480283", InternshipPeriod = "", Information="89A, Gamebreaker Malta, Triq Fleur-de-Lys, Birkirkara. 9 till 5" },
                new Company { CompanyName = "Gerada Pharmacy", Sector = "Pharmacy", PersonInCharge = "Justin Cassar", Email = "gerada@jvpharma.eu", ContactNumber = "+356 2180 6009", InternshipPeriod = "", Information="Triq Il-Madonna Tal-Bon Kunsill, I?-?ejtun" },
                new Company { CompanyName = "Hand in Hand Ltd.", Sector = "Services", PersonInCharge = "Emily Vassallo", Email = "emily.vassallo@handinhand.mt", ContactNumber = "+356 21456022", InternshipPeriod = "", Information="8, Triq Zondadari, Rabat, Malta RBT 1173" },
                new Company { CompanyName = "Hot Yoga and Pilates (HYP)", Sector = "Wellness", PersonInCharge = "Luca Blandi", Email = "hypmalta@gmail.com", ContactNumber = "+356 7957 9794", InternshipPeriod = "", Information="19 Sant' Agata Tas-Sliema, SLM 1015" },
                new Company { CompanyName = "Hugos Hotel", Sector = "Hospitality", PersonInCharge = "Mohsin Ali Mirza", Email = "mohsin.alimirza@hugosmalta.com", ContactNumber = "+356 2016 2400", InternshipPeriod = "", Information="St.George's Court, St.Augustine Street, St.Julian's. 35 hours, 5 days a week" },
                new Company { CompanyName = "Head Rush", Sector = "Hair & Beauty", PersonInCharge = "Kathleen Scerri", Email = "hairdressingheadrush@gmail.com", ContactNumber = "(00356)27202879", InternshipPeriod = "", Information="Headrush Hairdressing, Sliema. Tue-Sat: 9:00 am to 5:00 pm" },
                new Company { CompanyName = "Horizon2000", Sector = "Marketing", PersonInCharge = "Martin Sultana", Email = "horizon2000computers@gmail.com", ContactNumber = "+356 2746 2236", InternshipPeriod = "", Information="23, Triq De Rohan ,Zebbug, ZBG 2073. 9 am to 5 pm." },
                new Company { CompanyName = "University of Malta - AI", Sector = "Education/IT", PersonInCharge = "Matthew Montebello", Email = "matthew.montebello@um.edu.mt", ContactNumber = "+356 2707 2652", InternshipPeriod = "", Information="tal-Qroqq msida, MSD 2080. 9 am to 5 pm." },
                new Company { CompanyName = "Harbour Solutions", Sector = "IT", PersonInCharge = "Joseph Cauchi", Email = "joseph.cauchi@hsl.mt", ContactNumber = "21434290", InternshipPeriod = "", Information="The Meridian, Triq L-Ghajn tan-Nofs, Mriehel Malta." },
                new Company { CompanyName = "Health Point", Sector = "Pharmacy", PersonInCharge = "Michelle Marie Abela", Email = "healthpoint@jvpharma.eu", ContactNumber = "+356 2713 7926", InternshipPeriod = "", Information="34, Triq ?arenu Dalli, Bir?ebbu?a" },
                new Company { CompanyName = "Happy Shopper", Sector = "Retail", PersonInCharge = "Michael Poryelli", Email = "michael@hs.mt", ContactNumber = "356 2738 2209", InternshipPeriod = "", Information="Zamenhof, L-Imsida" },
                new Company { CompanyName = "IGM Lifts", Sector = "Engineering", PersonInCharge = "Isaac", Email = "info@igmlifts.com", ContactNumber = "+356 9949 7853", InternshipPeriod = "", Information="I G M Lift Supplies & Services Triq Moroni, Sliema" },
                new Company { CompanyName = "ISPY Projects", Sector = "Business", PersonInCharge = "Brian Portelli", Email = "info@ispyprojects.com", ContactNumber = "99161281", InternshipPeriod = "", Information="79, Armonija, Triq Geronimo Abos, Iklin." },
                new Company { CompanyName = "IOT LTD", Sector = "Engineering", PersonInCharge = "Ms. Christine", Email = "info@dli.com.mt", ContactNumber = "", InternshipPeriod = "", Information="Level 2, D-Hub, Triq Is-Salib Ta' L-Imriehel, Birkirkara. Roster Based." },
                new Company { CompanyName = "JMC Real Estate", Sector = "Real Estate", PersonInCharge = "MaryAnne", Email = "jmcestate@gmail.com", ContactNumber = "00356 79030819", InternshipPeriod = "", Information="123, Triq Ghajn Dwieli Paola" },
                new Company { CompanyName = "Jekran Ltd", Sector = "Industrial", PersonInCharge = "Christian Borg", Email = "info@jekran.com", ContactNumber = "00356 21663405", InternshipPeriod = "", Information="BLB009B, 1st Avenue, Bulebel Industrial Estate, Zejtun. 07:00 am till 15:30." },
                new Company { CompanyName = "JOHN G. CASSAR LIMITED", Sector = "Business", PersonInCharge = "Stephania Buhagiar", Email = "sbuhagiar@johngcassar.com", ContactNumber = "99802360", InternshipPeriod = "", Information="VICTORY STREET, QORMI" },
                new Company { CompanyName = "James Caterers", Sector = "Catering", PersonInCharge = "Steve Garrette", Email = "", ContactNumber = "+3569988 8949", InternshipPeriod = "", Information="BULEBEL INDUSTRIAL ESTATE Zejtun" },
                new Company { CompanyName = "JPC Electrical", Sector = "Electrical", PersonInCharge = "Jean Paul Camilleri", Email = "JPC590@HOTMAIL.COM", ContactNumber = "+35679086997", InternshipPeriod = "", Information="83, Roseanne Carini Street Santa Venera SVR 1405" },
                new Company { CompanyName = "LEISURE STORES LIMITED", Sector = "Retail", PersonInCharge = "Daniela Duca", Email = "juniorscenterparc@leisurestores.com", ContactNumber = "+356 79274785", InternshipPeriod = "", Information="Pama Shopping Village / Centerparc. 35 hours." },
                new Company { CompanyName = "L-Artigjana", Sector = "Retail", PersonInCharge = "Josianne Calleja", Email = "lartigjana@gmail.com", ContactNumber = "79473807", InternshipPeriod = "", Information="73 St Rita Street Rabat malta. Mon-Sat 10am-7pm." },
                new Company { CompanyName = "Little sunbeams", Sector = "Childcare", PersonInCharge = "Marion Houghton", Email = "info@littlesunbeams.com.mt", ContactNumber = "+356 99504055", InternshipPeriod = "", Information="triq San Giljan Birkirkara. 40hrs." },
                new Company { CompanyName = "MOTION BLUR LTD", Sector = "Media", PersonInCharge = "Shirley Spiteri Mintoff", Email = "Shirley@motionblur.com.mt", ContactNumber = "21441780", InternshipPeriod = "", Information="5, TONI BAJJADA SQUARE, NAXXAR" },
                new Company { CompanyName = "MYCOMMUNICATIONS LTD", Sector = "Media", PersonInCharge = "Nicola Mancuso", Email = "nicola@myc.com.mt", ContactNumber = "0035627294969", InternshipPeriod = "", Information="Mompalau Buildings, Level 2, Tower Road MSIDA" },
                new Company { CompanyName = "Marion Mizzi Wellbeing", Sector = "Wellness", PersonInCharge = "Petra Vella", Email = "petra@marionmizzi.com", ContactNumber = "+356 2262 3421", InternshipPeriod = "", Information="High Street, Tas-Sliema SLM 1542" },
                new Company { CompanyName = "Maltitask Ltd", Sector = "Services", PersonInCharge = "Mr. Prashad", Email = "info@maltitask.com", ContactNumber = "+35699745571", InternshipPeriod = "", Information="68 Triq San Frangisk , Fgura. 9 am to 5 pm." },
                new Company { CompanyName = "M4 Pharmacy", Sector = "Pharmacy", PersonInCharge = "Anna", Email = "m4pharmacy@gmail.com", ContactNumber = "+356 2143 6531", InternshipPeriod = "", Information="M4 Shopping Complex, Triq il-Linja, Attard" },
                new Company { CompanyName = "M E Electronics", Sector = "Electronics", PersonInCharge = "Mike Mangion", Email = "mikemangion@gmail.com", ContactNumber = "+35699 476 693", InternshipPeriod = "", Information="4, Mangion House Triq il-Hammieri Qormi" },
                new Company { CompanyName = "MALTA THEMED TOURS", Sector = "Tourism", PersonInCharge = "Giacomo Muscat", Email = "discover@maltathemedtours.com", ContactNumber = "77058201", InternshipPeriod = "", Information="63c, Flt 1, Birkirkara Road, St Julians. 35-40 hours" },
                new Company { CompanyName = "MOSTA TECH", Sector = "Engineering", PersonInCharge = "Alison Agius", Email = "alison.agius@mostatech.mt", ContactNumber = "21800557", InternshipPeriod = "", Information="F13, Leiden Centre, Mosta Technopark, Mosta. 06:30 till 12:30." },
                new Company { CompanyName = "NRGY Concepts Ltd", Sector = "Engineering", PersonInCharge = "Jonathan Pace", Email = "info@nrgy.com.mt", ContactNumber = "+356 9980 5029", InternshipPeriod = "", Information="3 Triq Bisbut, ?ejtun. Smart Casual." },
                new Company { CompanyName = "Nina Covaci Malta", Sector = "Retail", PersonInCharge = "Adriana", Email = "contact@ninacovaci.com", ContactNumber = "35627112718", InternshipPeriod = "", Information="Namaste Court, 81 Naxxar Road, Birkirkara" },
                new Company { CompanyName = "Ohea Pharmacy", Sector = "Pharmacy", PersonInCharge = "Martina Wood", Email = "oheapharmacy@gmail.com", ContactNumber = "+356 7941 7026", InternshipPeriod = "", Information="115 Triq Manoel Vilhena, G?ira" },
                new Company { CompanyName = "Pearl Spas", Sector = "Wellness", PersonInCharge = "Jessica Caruana", Email = "jcaruana@pearlspas.com", ContactNumber = "+356 23503705", InternshipPeriod = "", Information="Triq it-Turisti | Qawra" },
                new Company { CompanyName = "Perfect Look", Sector = "Hair & Beauty", PersonInCharge = "Trevor Cassar", Email = "trevorcassar@yahoo.com", ContactNumber = "+356 9944 6191", InternshipPeriod = "", Information="Hamrun Organic hair salon" },
                new Company { CompanyName = "St. Bartholomeo Pharmacy", Sector = "Pharmacy", PersonInCharge = "Mathea Montebello", Email = "math.montebello@gmail.com", ContactNumber = "+356 2148 2558", InternshipPeriod = "", Information="St. Bartholomeo Street, Santa Venera" },
                new Company { CompanyName = "Pompei Pharmacy", Sector = "Pharmacy", PersonInCharge = "Francesca Azzopardi", Email = "pompeipharmacy@jvpharma.eu", ContactNumber = "+356 2165 1278", InternshipPeriod = "", Information="120 Xatt is-Sajjieda, Marsaxlokk" },
                new Company { CompanyName = "PRODENT", Sector = "Dental Hygiene", PersonInCharge = "Dr. Lara Cutajar Cassar", Email = "", ContactNumber = "99944444", InternshipPeriod = "", Information="164, Naxxar Road San Gwann" },
                new Company { CompanyName = "Radisson Blu Resort", Sector = "Hospitality", PersonInCharge = "Danny Van Krugten", Email = "hr.goldensands@rdbmalta.com", ContactNumber = "+356 2356 1000", InternshipPeriod = "", Information="Golden Sands, Mellie?a MLH 5510" },
                new Company { CompanyName = "Retail International Group", Sector = "Retail", PersonInCharge = "Emerah Adak", Email = "emrah.adak@retail.com.mt", ContactNumber = "+356 2557 0000", InternshipPeriod = "", Information="4, Lascaris Buildings Lascaris Wharf Valletta. 9:00 am to 5:00 pm" },
                new Company { CompanyName = "Ristorante La Vela", Sector = "Retail", PersonInCharge = "", Email = "", ContactNumber = "", InternshipPeriod = "", Information="Wines & Spirits Retail, Pieta" },
                new Company { CompanyName = "St Julians Bay hotel", Sector = "Hospitality", PersonInCharge = "Hector Tovar", Email = "marina@eshotelsmalta.com", ContactNumber = "2133 6461", InternshipPeriod = "", Information="Tigne Sea Front, Sliema SLM 3010" },
                new Company { CompanyName = "SALTWAVE LTD", Sector = "Media", PersonInCharge = "Jonathan Mizzi", Email = "info@cargo.com.mt", ContactNumber = "99474699", InternshipPeriod = "", Information="Birgu Waterfront" },
                new Company { CompanyName = "Sophisticut Salon", Sector = "Hair & Beauty", PersonInCharge = "Georgette Galea", Email = "sophisticut2006@hotmail.com", ContactNumber = "2132 3539", InternshipPeriod = "", Information="Triq Tal-Qroqq, Msida. Tue-Sat 9-5." },
                new Company { CompanyName = "Sheron Pace Salon", Sector = "Hair & Beauty", PersonInCharge = "Clyde Pace", Email = "heandshehairsalon@hotmail.com", ContactNumber = "+35679447318", InternshipPeriod = "", Information="7 Sliema Road, Gzira. Tue-Sat 9-6." },
                new Company { CompanyName = "SEBASEMA CHILDCARE", Sector = "Childcare", PersonInCharge = "Marilyn Meli", Email = "info@sebasema.com", ContactNumber = "+356 79828194", InternshipPeriod = "", Information="Seba' Sema Childcare, Triq Ganu, Birkirkara. 8:00am - 3:30pm" },
                new Company { CompanyName = "SQUAREDEAL", Sector = "Retail", PersonInCharge = "Chiara Stefano", Email = "cg@squaredeal.com.mt", ContactNumber = "20107094", InternshipPeriod = "", Information="Level 1, Melita Court, Giuseppe Cali, Ta' Xbiex. 35 hours" },
                new Company { CompanyName = "Smurf’s Village", Sector = "Childcare", PersonInCharge = "Violeta Andrevska", Email = "smurfsvillage2023@gmail.com", ContactNumber = "+356 9909 5979", InternshipPeriod = "", Information="26, Noura maisonette Triq Sammy Calleja Mosta. 9am-3pm" },
                new Company { CompanyName = "Screen Salon", Sector = "Hair & Beauty", PersonInCharge = "Antoine Buhagiar", Email = "louantbuhagiar@gmail.com", ContactNumber = "21570768", InternshipPeriod = "", Information="71 triq il-bardnell bugibba" },
                new Company { CompanyName = "Sghendo Woodworks", Sector = "Woodworks", PersonInCharge = "", Email = "", ContactNumber = "+356 9922 9600", InternshipPeriod = "", Information="Triq Mastru Gorg Cachia, Tal-Handaq, Qormi" },
                new Company { CompanyName = "The Marigold Foundation", Sector = "NGO", PersonInCharge = "Mariella Agius", Email = "mariella@rarediseasesmalta.com", ContactNumber = "99302365", InternshipPeriod = "", Information="58 ZACHARY STREET, VALLETTA" },
                new Company { CompanyName = "The Black Sheep", Sector = "Hospitality", PersonInCharge = "Zinaida Sabeva", Email = "theblacksheep.malta@gmail.com", ContactNumber = "99745571", InternshipPeriod = "", Information="55 Triq Ix - Xatt Tas-Sliema Sliema. Roster Based." },
                new Company { CompanyName = "Valletta Lucente", Sector = "Hospitality", PersonInCharge = "Dorothy Cordina", Email = "seeyou@vallettalucente.com", ContactNumber = "99452318", InternshipPeriod = "", Information="20b, St. Lucia Street, Valletta" },
                new Company { CompanyName = "Wadge studio salon", Sector = "Hair & Beauty", PersonInCharge = "Clint Abela Wadge", Email = "vagguwadge@gmail.com", ContactNumber = "+356 79091533", InternshipPeriod = "", Information="WADGE STUDIO, Triq tal-Qattus, Birkirkara. Tue-Fri 9-6." }
            };

            foreach (var company in companies)
            {
                PrepareCompany(company);
            }

            db.Companies.AddRange(companies);
            db.SaveChanges();
            EnsureRequiredCompanies(db);
        }

        private static void EnsureRequiredCompanies(AppDbContext db)
        {
            var company = db.Companies.FirstOrDefault(c => c.CompanyName == "3Plex Group");

            if (company is null)
            {
                company = new Company
                {
                    CompanyName = "3Plex Group",
                    Sector = "Aviation",
                    PersonInCharge = "Paul Fenech",
                    Email = "hr@3plexgroup.com",
                    ContactNumber = "+356 99036205",
                    InternshipPeriod = "-",
                    Information = "C78062"
                };

                db.Companies.Add(company);
            }
            else
            {
                company.Sector = "Aviation";
                company.PersonInCharge = "Paul Fenech";
                company.Email = "hr@3plexgroup.com";
                company.ContactNumber = "+356 99036205";

                if (string.IsNullOrWhiteSpace(company.Information))
                {
                    company.Information = "C78062";
                }
            }

            db.SaveChanges();
        }

        private static void PrepareCompany(Company company)
        {
            var rawSource = !string.IsNullOrWhiteSpace(company.InternshipPeriod)
                ? company.InternshipPeriod
                : company.Information;

            company.InternshipPeriod = CompanyTextHelper.GetInternshipPeriodDisplay(rawSource);
            company.Information = CompanyTextHelper.GetAdditionalInformation(rawSource);
        }

        private static void NormalizeExistingCompanies(AppDbContext db)
        {
            var companies = db.Companies.ToList();
            var hasChanges = false;

            foreach (var company in companies)
            {
                var rawSource = string.Join(". ", new[] { company.InternshipPeriod, company.Information }
                    .Where(value => !string.IsNullOrWhiteSpace(value)));

                if (string.IsNullOrWhiteSpace(rawSource))
                {
                    continue;
                }

                var normalizedPeriod = CompanyTextHelper.GetInternshipPeriodDisplay(rawSource);
                var information = CompanyTextHelper.GetAdditionalInformation(rawSource);

                if (normalizedPeriod != company.InternshipPeriod || information != company.Information)
                {
                    company.InternshipPeriod = normalizedPeriod;
                    company.Information = information;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                db.SaveChanges();
            }
        }

        private static void EnsureInformationColumn(AppDbContext db)
        {
            if (ColumnExists(db, "Companies", "Information"))
            {
                return;
            }

            db.Database.ExecuteSqlRaw("ALTER TABLE Companies ADD COLUMN Information TEXT NULL;");
        }

        private static bool ColumnExists(AppDbContext db, string tableName, string columnName)
        {
            using var connection = db.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName});";
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (string.Equals(reader[1]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
