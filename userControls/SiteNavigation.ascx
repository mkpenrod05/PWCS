<%@ Control Language="VB" ClassName="SiteNavigation" %>

<div id="SiteNavigation" style="margin-bottom:20px;"></div>

<script type="text/javascript">

$("#SiteNavigation").kendoMenu({
    //open: onOpen,
    //close: onClose,
    dataSource: [
            {
                text: "Home",
                url: "Default.aspx",
                spriteCssClass: "ui-icon ui-icon-home",
            },
            {
                text: "Transfers",
                url: "transfer.aspx",
                spriteCssClass: "ui-icon ui-icon-transferthick-e-w"
            },
            {
                text: "Archive",
                url: "archive.aspx",
                spriteCssClass: "ui-icon ui-icon-suitcase"
            },
            {
                text: "Maintenance",
                url: "maintenance.aspx",
                spriteCssClass: "ui-icon ui-icon-wrench"
            },
            {
                text: "Modifications",
                url: "modifications.aspx",
                spriteCssClass: "ui-icon ui-icon-gear"
            },
            {
                text: "Dashboard",
                url: "dashboard.aspx?query=Managers",
                spriteCssClass: "ui-icon ui-icon-note",
                items: [
                    {
                        text: "Account Managers",
                        url: "dashboard.aspx?query=Managers"
                    },
                    {
                        text: "Archived Assets",
                        url: "dashboard.aspx?query=Archive"
                    },
                    {
                        text: "Assets Unaccounted For",
                        url: "dashboard.aspx?query=UnaccountedFor"
                    },
                    {
                        text: "All Open Trunk ID's",
                        url: "dashboard.aspx?query=AllOpenTrunkID"
                    }
                ]
            }
        ]
});

</script>