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
    section [Class "section research"; Id "research"] [
        createFrame [|ExampleBase; Example2; Example3; Example4|]
    ]