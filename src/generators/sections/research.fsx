#r "../../_lib/Fornax.Core.dll"

open Html

type FrameSlide = {
    Content: string
    Image: string
}

let Example1 = {
  Content = """<ul>
  <li>Main content categories, which describe common rules shared by many elements.</li>
  <li>Form-related content categories, which describe rules common to form-related elements.</li>
  <li>Specific content categories, which describe rare categories shared only by a few elements, sometimes only in a specific context.</li>
</ul>"""
  Image = "https://picsum.photos/200/300"
}

let Example2 = {
  Content = """<p>Flow content is a broad category that encompasses most elements that can go inside the <a href="/en-US/docs/Web/HTML/Element/body"><code>&lt;body&gt;</code></a> element, including heading elements, sectioning elements, phrasing elements, embedding elements, interactive elements, and form-related elements. It also includes text nodes (but not those that only consist of white space characters).</p>"""
  Image = "https://picsum.photos/200/300"
}
let Example3 = {
  Content = """<section aria-labelledby="sectioning_content"><h3 id="sectioning_content"><a href="#sectioning_content">Sectioning content</a></h3><div class="section-content"><p>Sectioning content, a subset of flow content, creates a <a href="/en-US/docs/Web/HTML/Element/Heading_Elements">section in the current outline</a> defining the scope of <a href="/en-US/docs/Web/HTML/Element/header"><code>&lt;header&gt;</code></a> and <a href="/en-US/docs/Web/HTML/Element/footer"><code>&lt;footer&gt;</code></a> elements.</p>
<p>Elements belonging to this category are <a href="/en-US/docs/Web/HTML/Element/article"><code>&lt;article&gt;</code></a>, <a href="/en-US/docs/Web/HTML/Element/aside"><code>&lt;aside&gt;</code></a>, <a href="/en-US/docs/Web/HTML/Element/nav"><code>&lt;nav&gt;</code></a>, and <a href="/en-US/docs/Web/HTML/Element/section"><code>&lt;section&gt;</code></a>.</p></div></section>"""
  Image = "https://picsum.photos/200/300"
}
let Example4 = {
  Content = """<ul>
  <li><a href="/en-US/docs/Web/HTML/Element/a"><code>&lt;a&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/a#href"><code>href</code></a> attribute is present</li>
  <li><a href="/en-US/docs/Web/HTML/Element/audio"><code>&lt;audio&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/audio#controls"><code>controls</code></a> attribute is present</li>
  <li><a href="/en-US/docs/Web/HTML/Element/img"><code>&lt;img&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/img#usemap"><code>usemap</code></a> attribute is present</li>
  <li><a href="/en-US/docs/Web/HTML/Element/input"><code>&lt;input&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/input#type">type</a> attribute is not in the hidden state</li>
  <li><a href="/en-US/docs/Web/HTML/Element/object"><code>&lt;object&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/object#usemap"><code>usemap</code></a> attribute is present</li>
  <li><a href="/en-US/docs/Web/HTML/Element/video"><code>&lt;video&gt;</code></a>, if the <a href="/en-US/docs/Web/HTML/Element/video#controls"><code>controls</code></a> attribute is present</li>
</ul>"""
  Image = "https://picsum.photos/200/300"
}


let private createFrameSlide (slide: FrameSlide) =
  let innerFlexBoxContainer = HtmlProperties.Style [Display "flex"; CSSProperties.Height "100%"; AlignItems "center"]
  li [Class "splide__slide"] [
      // container
      div [Class "is-flex fixed-grid research__bg"; HtmlProperties.Style [CSSProperties.Height "100%"; AlignItems "center"; JustifyContent "center"]] [
          div [Class "grid research__grid"; HtmlProperties.Style [CSSProperties.Custom("justify-items", "end"); CSSProperties.Height "fit-content";]] [
              div [Class "cell research__cell"] [
                div [innerFlexBoxContainer] [
                  div [] [
                    img [Src slide.Image]
                  ]
                ]
              ]
              div [Class "cell research__cell"] [
                div [innerFlexBoxContainer] [
                  div [Class "content research__content"] [!!slide.Content]
                ]
              ]
          ]
      ]
  ]

let generate (ctx : SiteContents) (_: string) =
    // section [Class "section research"; Id "research"] [
    //     // createFrame [|ExampleBase; Example2; Example3; Example4|]
        
    // ]
    let slides = [Example1; Example2; Example3; Example4]
    div [HtmlProperties.Style [CSSProperties.Height "400px"]] [
        section [Class "splide"; HtmlProperties.Custom("aria-label","current research information")] [
            // div [Class "splide__slider"] [
            // ]
            div [Class "splide__track"] [
                ul [Class "splide__list"] [
                    for slide in slides do
                      createFrameSlide slide
                ]
            ]
            div [Class "splide__progress"] [
                div [Class "splide__progress__bar"] []
            ]
            // https://github.com/Splidejs/splide/issues/1310
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
