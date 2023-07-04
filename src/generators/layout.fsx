#r "../_lib/Fornax.Core.dll"


open Html

let rptu_navbar() =
    nav [Role "navigation"; Class "rptu-navbar"; Id "rptu-navbar"] [
        div [Class "container is-flex is-flex-direction-column"] [
            div [Class "is-flex mb-3"] [
                a [Class "rptu-logo"; Href "" ] [
                    img [ Src "./content/images/rptu_web_logo_schwarz.svg" ]
                ]
                ul [Class "rptu-container"; ] [
                    li [] [
                        span [Class "icon is-small has-text-link"] [i [Class "fa-solid fa-envelope"] []]
                        a [Href "#contact"] [!!"Contact"]
                    ]
                ]
            ]
            h2 [Class "rptu-department"] [
                a [Href "https://bio.rptu.de"; HtmlProperties.Style [CSSProperties.MarginLeft "-1px"]] [!!"Department of Biology"]
            ]
        ]
    ]

let csb_navbar() =
    nav [Role "navigation"; Class "navbar is-light"; Id "csb-navbar"] [
        div [Class "navbar-brand"] [
            a [Class "navbar-item"] [
                img [Src "./content/images/logo_small.png"]
            ]
            div [Class "navbar-burger"; Role "button"; HtmlProperties.Custom("data-target","navMenu")] [
                span [HtmlProperties.Custom("aria-hidden", "true")] []  
                span [HtmlProperties.Custom("aria-hidden", "true")] []  
                span [HtmlProperties.Custom("aria-hidden", "true")] []  
            ]
        ]
        div [Class "navbar-menu"; Id "navMenu"] [
            a [Class "navbar-item"; Href "#research"] [
                !!"Research"
            ]
            a [Class "navbar-item"; Href "#team"] [
                !!"Team"
            ]
            a [Class "navbar-item"; Href "#publication"] [
                !!"Publications"
            ]
            a [Class "navbar-item"; Href "#teaching"] [
                !!"Teaching"
            ]
            a [Class "navbar-item"; Href "#contact"] [
                !!"Contact"
            ]
        ]
    ]

let scaffold (ctx : SiteContents) bodyCnt =
    html [] [
        head [] [
            meta [CharSet "utf-8"]
            title [] [!! "CSB - RPTU Kaiserslautern"]
            link [Rel "icon"; Type "image/png"; Href "./content/images/favicon.png"]
            meta [Name "viewport"; Content "width=device-width, initial-scale=1"]
            meta [Name "description"; Content "CSB (Computational Systems Biology) at RPTU Kaiserslautern, Germany"]
            // link [Rel "stylesheet"; Type "text/css"; Href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css"]
            link [Rel "stylesheet"; Href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.3.0/css/all.min.css"; Integrity "sha512-SzlrxWUlpfuzQ+pcUCosxcglQRNAq/DZjVsC0lE40xsADsfeQoEypE+enwcOiGjk/bSuGGKHEyjSoQ1zVisanQ=="; CrossOrigin "anonymous"; HtmlProperties.Custom("referrerpolicy", "no-referrer")]
            script [Defer true; Src "https://kit.fontawesome.com/0d3e0ea7a6.js"; CrossOrigin "anonymous"] []
            link [Rel "stylesheet"; Type "text/css"; Href "style/css/main.css"]
        ]
        body [] [
            rptu_navbar()
            csb_navbar()
            main [Class "csb-content"] [
                // bodyCnt
                yield! bodyCnt
            ]
            
            //<!-- jQuery Slim -->
            script [Src "//ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"] []
            script [] [!! """window.jQuery || document.write('<script src="assets/bower_components/jquery/dist/jquery.min.js"><\/script>')"""] 
            
            //<!-- jQuery Plugins -->
            script [Src "style/bower_components/jquery-unveil/jquery.unveil.min.js"] []
            //<!-- Main Js -->
            script [Src "style/js/main.js"] []

            //   <!-- Webfont Loader -->
            script [] [
                        !! """
                            WebFontConfig = {
                                google: {
                                    families: ['Montserrat:400,700', 'Lora:400,400i,700']
                                },
                                custom: {
                                    families: ['FontAwesome'],
                                    urls: ['https://cdn.jsdelivr.net/fontawesome/4.7.0/css/font-awesome.min.css']
                                }
                            };

                            (function(d) {
                                var wf = d.createElement('script'), s = d.scripts[0];
                                wf.src = 'https://ajax.googleapis.com/ajax/libs/webfont/1.6.16/webfont.js';
                                s.parentNode.insertBefore(wf, s);
                            })(document);
                        """
            ]
        ]  
    ]
        
    

let render (ctx : SiteContents) cnt =
  //let disableLiveRefresh = ctx.TryGetValue<Postloader.PostConfig> () |> Option.map (fun n -> n.disableLiveRefresh) |> Option.defaultValue false
  cnt
  |> HtmlElement.ToString
  //|> fun n -> if disableLiveRefresh then n else injectWebsocketCode n
