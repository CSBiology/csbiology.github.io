#r "../_lib/Fornax.Core.dll"
#load "sections/welcome.fsx"
#load "sections/research.fsx"
#load "sections/team.fsx"
#load "sections/publications.fsx"
#load "sections/teaching.fsx"
#load "sections/contact.fsx"
#load "sections/foot.fsx"

#load "layout.fsx"


open Html


let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    [
        Welcome.generate ctx ""
        Research.generate ctx ""
        Team.generate ctx ""
        Publications.generate ctx ""
        Teaching.generate ctx ""
        Contact.generateGitHub ctx ""
        Contact.generate ctx ""
        Foot.generate ctx ""
    ]
    |> Layout.scaffold ctx  
    |> Layout.render ctx 