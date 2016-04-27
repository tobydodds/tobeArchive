using Orchard;
using Orchard.UI.Navigation;

namespace Tobe.Collection.Menus {
    public class AdminMenuGlossary : Component, INavigationProvider {
        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Glossary"), "5.1", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {
            menu.Add(T("Agents"), "1.1", item => item
                .Action("Index", "AgentsAdmin", new { area = "Tobe.Collection" }));
            menu.Add(T("Agents"), "1.2", item => item
                .Action("Index", "AgentsAdmin", new { area = "Tobe.Collection" }));
            menu.Add(T("Countries"), "1.3", item => item
                .Action("Index", "CountriesAdmin", new { area = "Tobe.Collection" }));
            menu.Add(T("Genres"), "1.5", item => item
                .Action("Index", "GenresAdmin", new { area = "Tobe.Collection" }));
            menu.Add(T("Record Labels"), "1.8", item => item
                .Action("Index", "RecordLabelsAdmin", new { area = "Tobe.Collection" }));
            menu.Add(T("Roles"), "1.9", item => item
                .Action("Index", "RolesAdmin", new { area = "Tobe.Collection" }));
        }
    }
}