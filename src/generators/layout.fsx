#r "../_lib/Fornax.Core.dll"


open Html

let scaffold (ctx : SiteContents) bodyCnt =
    html [] [
        head [] [
            meta [CharSet "utf-8"]
            title [] [!! "CSB - RPTU Kaiserslautern"]
            link [Rel "icon"; Type "image/png"; Href "./content/images/favicon.png"]
            meta [Name "viewport"; Content "width=device-width, initial-scale=1"]
            meta [Name "description"; Content "CSB (Computational Systems Biology) at RPTU Kaiserslautern, Germany"]
            script [Defer true; Src "https://kit.fontawesome.com/0d3e0ea7a6.js"; CrossOrigin "anonymous"] []
            link [Rel "stylesheet"; Type "text/css"; Href "style/css/main.css"]
        ]
        body [] [
          header [Class "header"] [
            div [Class "container"] [
              //Title "CSB - Computational Systems Biology"
              a [Class "logo"; Href "/";] [ span [] [!! "CSB - Computational Systems Biology"] ]
              
              nav [Class "nav"] [
                ul [] [
                  li [] [ a [Href "#research"] [ 
                    i [Class "fa fa-flask"; HtmlProperties.Custom ("aria-hidden", "true") ] [];
                      !! "Research"]]
                  li [] [ a [Href "#team"] [ 
                    i [Class "fa fa-users"; HtmlProperties.Custom ("aria-hidden", "true") ] [];
                      !! "Team"]]
                  li [] [ a [Href "#publication"] [ 
                    i [Class "fa fa-book"; HtmlProperties.Custom ("aria-hidden", "true") ] [];
                      !! "Publications"]]
                  li [] [ a [Href "#teaching"] [ 
                    i [Class "fa fa-graduation-cap"; HtmlProperties.Custom ("aria-hidden", "true") ] [];
                      !! "Teaching"]]
                  li [] [ a [Href "#contact"] [ 
                    i [Class "fa fa-globe"; HtmlProperties.Custom ("aria-hidden", "true") ] [];
                      !! "Contact"] ]
                ]
              ]
            ] 
          ]
          main [Class "content"] [
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
                      """]
        ]  
      ]
        
    

let render (ctx : SiteContents) cnt =
  //let disableLiveRefresh = ctx.TryGetValue<Postloader.PostConfig> () |> Option.map (fun n -> n.disableLiveRefresh) |> Option.defaultValue false
  cnt
  |> HtmlElement.ToString
  //|> fun n -> if disableLiveRefresh then n else injectWebsocketCode n
