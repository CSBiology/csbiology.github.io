#r "../../_lib/Fornax.Core.dll"

open Html

type FrameSlide = {
    Title: string
    Subtitle: string
    Text: string
    BackgroundColor: string
}

let ExampleBase = {
  Title = "Welcome to CSB"
  Subtitle = "What our research is aiming for"
  Text = "The main focus of our group is the application and development of computational methods to process and integrate quantitative biological data from modern high-throughput measurements in order to gain novel insights into biological responses to environment changes. The main challenge is the rigorous integration of different system level analyses and present knowledge into biological interpretable models. Therefore, we want to drive theory and technology forward with a combination of biological science, applied informatics, statistical and machine learning approaches."
  BackgroundColor = "#ed7d31"
}

let Example2 = {ExampleBase with Title = "Example 2"; BackgroundColor = "#5b9bd5"}
let Example3 = {ExampleBase with Title = "Example 3"; BackgroundColor = "#5b9bd5"}
let Example4 = {ExampleBase with Title = "Example 4"; BackgroundColor = "#5b9bd5"}

// ------------ ⚠️ -----------
// Must set the number of slides in scss: `style/scss/sections/_slider.scss` on top variable `$count-slides: 4;`
// ------------ ⚠️ -----------
let private createFrameSlide (slide: FrameSlide) (index: int) =
  let classFromindex index = sprintf "frame frame_%i" (index+1)
  div [Class (classFromindex index)] [
      div [Class "frame-content"] [
          div [Class "container container--full"] [
              div [Class "research__title"; HtmlProperties.Style [CSSProperties.BackgroundColor slide.BackgroundColor]] [
                  div [] [
                      h2 [] [ !! slide.Title ];
                      p [] [ !! slide.Subtitle ];
                  ]
              ];
          
              div [Class "research__text"] [
                  p [] [!! slide.Text]
              ]
          ] 
      ]
  ]

let private createRadioButton(index: int, createId: int -> string) =
    input [
        Type "radio"
        Id (createId index)
        Name "frame"
    ]

let private createFrame(slides: FrameSlide []) = 
    let idFromIndex index = sprintf "frame%i" (index+1)
    div [Id "frame"] [
        for i in 0..slides.Length-1 do
            createRadioButton(i, idFromIndex)
        div [Id "slides"] [
            div [Id "overflow"] [
                div [Class "inner"] [
                    for i in 0..slides.Length-1 do
                        createFrameSlide slides.[i] i
                ]
            ]
        ]
        div [Id "controls"] [
            for i in 0..slides.Length-1 do
                label [HtmlProperties.Custom("for", idFromIndex i)] []
        ]
        div [Id "bullets"] [
            for i in 0..slides.Length-1 do
                label [HtmlProperties.Custom("for", idFromIndex i)] []
        ]
    ]


let generate (ctx : SiteContents) (_: string) =
    // section [Class "section research"; Id "research"] [
    //     // createFrame [|ExampleBase; Example2; Example3; Example4|]
        
    // ]
    div [HtmlProperties.Style [CSSProperties.Height "600px"]] [
        section [Class "splide"; HtmlProperties.Custom("aria-label","current research information")] [
            // div [Class "splide__slider"] [
            // ]
            div [Class "splide__track"] [
                ul [Class "splide__list"] [
                    li [Class "splide__slide"] [!!"Slide 01"]
                    li [Class "splide__slide"] [!!"Slide 02"]
                    li [Class "splide__slide"] [!!"Slide 03"]
                ]
            ]
            div [Class "splide__progress"] [
                div [Class "splide__progress__bar"] []
            ]
            button [Class "splide__toggle"; Type "button"] [
                svg [Class "splide__toggle__play"; HtmlProperties.Custom("viewBox","0 0 24 24"); HtmlProperties.Custom("xmlns", "http://www.w3.org/2000/svg")] [
                    path  [HtmlProperties.Custom("d", "m22 12-20 11v-22l10 5.5z")] []
                ]
                svg [Class "splide__toggle__pause"; HtmlProperties.Custom("viewBox","0 0 24 24"); HtmlProperties.Custom("xmlns", "http://www.w3.org/2000/svg")] [
                    path  [HtmlProperties.Custom("d", "m2 1v22h7v-22zm13 0v22h7v-22z")] []
                ]
            ]
        ]
        script [] [
            !!"""document.addEventListener( 'DOMContentLoaded', function() {
    var splide = new Splide( '.splide', { type: 'loop', autoplay: true } );
    splide.mount();
} );"""
        ]
    ]
