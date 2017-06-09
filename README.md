# CNTKSharp

The project CNTKSharp is an early attempt by Lokad to illustrate what a .NET-first and developer-first approach built on top of [CNTK](https://github.com/Microsoft/CNTK) would mean.  We believe that the .NET high level instrumentation for deep learning should go well beyond mere C++ bindings. 

Our top goals for a deep learning powered .NET are:

1. correctness by design
2. high performance I/O
3. production and maintenance

At present time, CNTKSharp is early functional .NET/C# spike that only starting to address point No1. Our approach is based on the experience we have acquired at [Lokad](https://www.lokad.com) which has been extensively relying on both .NET and machine learning for nearly a decade.

## Why correctness by design

Machine learning is complicated, and complications are _bad_ as far production is concerned. For most .NET apps, budgets and deadlines are tight, and developers don't have the time chasing obscure problems. This is why _strong typing_ shines for complex applications: strong typing eliminates entire classes of problems which _could_ happen in production. Then, strong typing is only one of the many angles to achieving more _correctness by design_. Other languages like Rust or Closure provides other types of guarantees.

For a .NET-first deep learning, we want an approach that deliver _correctness by design_ to the greatest extend, as the upsides for productivity, reliability and accuracy are massive.

## Overview of CNTKSharp

The library delivers a C# parser and compiler of [BrainScript](https://docs.microsoft.com/en-us/cognitive-toolkit/BrainScript-Basic-Concepts), the DSL (domain-specific language) designed for CNTK.

More specifically, with CNTKSharp,  you can:

* parse a BrainScript into to C# expression tree
* create an expression tree in C# and compile it to BrainScript

The code below illustrates how CNTKSharp works with a BrainScript example.

```
using System;

namespace Lokad.BrainScript.Example
{
    class Program
    {
        static void Main()
        {
            var script = @"
BrainScriptNetworkBuilder = {   # (we are inside the train section of the CNTK config file)
    SDim = 28*28 # feature dimension
    HDim = 256   # hidden dimension
    LDim = 10    # number of classes
    # define the model function. We choose to name it 'model()'.
    model (features) = {
        # model parameters
        W0 = ParameterTensor {(HDim:SDim)} ; b0 = ParameterTensor {HDim}
        W1 = ParameterTensor {(LDim:HDim)} ; b1 = ParameterTensor {LDim}
        # model formula
        r = RectifiedLinear (W0 * features + b0) # hidden layer
        z = W1 * r + b1                          # unnormalized softmax
    }.z
    # define inputs
    features = Input {SDim}
    labels   = Input {LDim} 
    # apply model to features
    z = model (features)
    # define criteria and output(s)
    ce   = CrossEntropyWithSoftmax (labels, z)  # criterion (loss)
    errs = ErrorPrediction         (labels, z)  # additional metric
    P    = Softmax (z)     # actual model usage uses this
    # connect to the system. These five variables must be named exactly like this.
    featureNodes    = (features)
    inputNodes      = (labels)
    criterionNodes  = (ce)
    evaluationNodes = (errs)
    outputNodes     = (P)
}
BrainScriptNetworkBuilder = {
    # STEP 1: load 1-hidden-layer model
    inModel = BS.Network.Load (""model.1.dnn"")
    # get its h1 variable --and also recover its dimension
    h1 = inModel.h1
    H = h1.dim
    # also recover the number of output classes
    M = inModel.z.dim
    # STEP 2: create the rest of the extended network as usual
            W2 = Parameter(H, H); b2 = Parameter(H)
    Wout = Parameter(M, H); bout = Parameter(M)
    h2 = Sigmoid(W2 * h1 + b2)
    z = Wout * h2 + bout
    ce = CrossEntropyWithSoftmax(labels, z, tag ='criterion')
}";

            Console.WriteLine(Parser.Parse(script).Print(false));
        }
    }
}
```

Based on the library CNTKSharp, it's straight forward to compose a deep learning task in C#, and to serialize the expression to a file, and then call `cntk.exe` passing the file as a command-line argument.

## Why high-level representation matter

Manually dealing with tensors is an error-prone way of doing deep-learning. From a .NET perspective, client apps should be able to define a deep learning learning in a much more structured way. It's precisely through a high-level representation that a satisfying degree of _correctness by design_ can be achieved.

For CNTK, there is a lot of misunderstanding around BrainScript. Some people are rejecting BrainScript arguing that they don't want to learn a new language. Yet, the reality is that **you either learn a new language or you learn a new API**. 

BrainScript - the API - happens to be a vast improvement _from a app developer perspective_ over muddling with tensors. BrainScript just happen to be the _reification_ of the high-level representation of a deep learning task.

While this is partly an opinion, we believe that BrainScript is a _really good_ high level representation. There are many ways to make it even better, but it's precisely this sort of capabilities that can make CNTK stand out against the TensorFlow crowd.

With CNTKSharp, you have the option of using C# only or to inline BrainScript bits in your C#, still enforcing that your BrainScript literals do _compile_. If you are only using C#, your C# code will just happen to be more verbose. As far verbosity is concerned, DSLs always win against APIs.

## High-level design should happen in .NET

The high level representation of the deep learning task, presently _reified_ through BrainScript, should be a .NET object.  Through .NET, it's straightforward to implement many high-level transformations and many high-level analysis of the deep learning task treated as an algebraic expression.

This logic should not happen in C++ through bindings because this is the near certainty to generate a significant development friction at the .NET level: lack of extensibility, poor exception stacks, limited static code analysis, etc. Thus, in CNTKSharp we have opted for a pure .NET/C# approach to the design of deep learning tasks.

**CNTKSharp does not even presently depends on the CNTK library.** Everything happens in .NET. Our vision is that coupling points between the high-level .NET and the low-level CNTK should be strategically chosen, and that overall, there will be very few coupling points.

## What next?

Our plans are fuzzy, but a few desirable elements:

* a .NET binding to skip the `cntk.exe` command line entirely
* a .NET high-perf I/O binding to avoid moving data around through files
* a .NET presentation of the trained model with (de)serialization
* bundling the .NET binding for model evaluation
* improvements on the DSL itself (only need to _compile_ to BrainScript).
