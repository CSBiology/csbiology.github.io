#r "../../_lib/Fornax.Core.dll"

open Html

let generate (ctx : SiteContents) (_: string) =
    section [Class "section welcome"] [
        div [Class "container"] [
            h2 [] [ !! "Welcome to CSB" ];
            p [] [ !! "The capability of biological systems to respond to environmental changes is realized by a complex dynamic adjustment of the interplay between genes, proteins and metabolites. For a deeper understanding at the systems level, we need to study the structure and dynamics of cellular and organismal functions rather than the characteristics of isolated parts of a cell or an organism."];
        ];
        div [Class "welcome__overlay"] [];
        video [ AutoPlay true; Loop true; Class "welcome__video"; Muted true ] [
            source [Src "./style/video/mp4/Working-Space.mp4"; Type "video/mp4"];
            source [Src "./style/video/ogv/Working-Space.ogv"; Type "video/mp4"];
            source [Src "./style/video/webm/Working-Space.webm"; Type "video/webm"];
            img [Src "./style/video/snapshots/Working-Space.jpg"; Alt ""] 
        ]    
    ]
    
    