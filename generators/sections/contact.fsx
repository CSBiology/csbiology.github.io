#r "../../_lib/Fornax.Core.dll"

open Html

let generateGitHub (ctx : SiteContents) (_: string) =
    section [Class "section github"; Id "github"] [
        div [Class "container"] [
            a [Href "https://github.com/CSBiology"; Class "github__link"] [
                i [Class "fa fa-github"; Hidden true] []
            ]
            
            div [Class "github__text"] [
                h3 [] [ !! "Open Source" ]
                p [] [!!"Take a look on our public repositories on github. Feel free to contribute!"]
            ]

            a [Href "https://github.com/CSBiology"; Class "btn"] [!!"View Github"]
        ]        
    ]

let generate (ctx : SiteContents) (_: string) =
    section [Class "section contact"; Id "contact"] [
        div [Class "container"] [
            div [] [
                h3 [] [ !! "Contact" ]
                a [Href "https://www.bio.uni-kl.de/biologie-in-kaiserslautern/"] [!!"Faculty"]
                a [Href "http://www.uni-kl.de/impressum/"] [!!"Impressum"]
                a [Href "http://www.uni-kl.de/datenschutzerklaerung/"] [!!"Datenschutz"]                
            ]
            p [] [
                !!"""Prof. Dr. Timo Mühlhaus<br>
                     Computational Systems Biology<br><br>
                     RPTU University of Kaiserslautern<br>
                     Paul-Ehrlich-Str. 23 R109<br>
                     67663 Kaiserslautern, Germany<br><br>
                """
                i [Class "fa fa-phone"] []; !!"+ 49 631 205 4657"
                br []
                i [Class "fa fa-fax"] []; !!"+ 49 631 205 3799"
            ]
        ]        
    ]
 
