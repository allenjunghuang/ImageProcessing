# Image Processing

This application demonstrate popular image processing algorithms with PCX formate images.

## Geometric Operations

- Rotation

  Compare backward and forward rotation.
  ![rotation](help/rotation.PNG)

- Zoom

  Zoom-in the image by averaging and zoom-out by subsampling.

## Point Processing

Histogram indecates the frequency of brightness value in image.

- Histogram Equalization

  Producing an image with equally distributed brightness levels over the whole brightness scale.
  ![histogram equalization](help/histogram-equalization.PNG)

- Histogram Specification

  The aim is to produce an image with desired distributed brightness levels over the whole brightness scale, as opposed to uniform distribution.  
  ![histogram specification](help/histogram-specification.PNG)

- Otsu Thresholding

  The algorithm assumes the image histogram is bimodal and finds the optimum threshold to separate the two classes. It is capable of separating a foreground from a background.

- Bit-plane Slicing

  Highlighting the contribution made to the total image appearance by specific bits. The higher-order shows the majority of the visually significant data, and lower-order presents subtle details.


## Spatial Filtering

The use of spatial masks for image processing usually is called spatial filtering. The mask also called filter, kernel, or window. Note that the values in a mask are called coefficients, not pixels.

Linear spatial filtering often is referred to as convolving a mask with an image. For nonlinear spatial filtering, the filtering operation is based conditionally on the values of the pixels in the neighborhood under consideration, but not explicitly use coefficients in the sum-of-products manner, such as median filtering.

- Lowpass Filter

  - Average
    
    A spatial averaging filter in which all coefficients are equal. By the way, averaging is analogous to integration, and sharpening can be accomplished by spatial differentiation.
    ![lowpass](help/lowpass.PNG)

  - Median

    A nonlinear spatial filter whose response is based on ordering the pixels in the area to be filtered. It is useful to reconstruct an image corrupted by impulse noise or salt-and-pepper noise.

- Highpass Filter

  The shape of the impulse response needed to implement a high-pass spatial filter indicates that the filter should have positive coefficients near its center, and negative coefficients in the outer periphery, such as Laplacian operator. 

- Edge Crispening

  Making edges slightly sharper and crisper. This operation is subtracting a blurred version of an image from the image itself.
  
  ![edge crispening](help/edge-crispening.PNG)

- Gradient Filter

  There are three popular operators:
  - Sobel

  ![gradient sobel](help/gradient-sobel.PNG)
  - Prewitt

  ![gradient prewitt](help/gradient-prewitt.PNG)
  - Roberts

  ![gradient roberts](help/gradient-roberts.PNG)

## Image Transformation

- Fractal

  If we consider a set to be fractal, we think of it as having the following properties:   
  - The set has detail at every scale and is self-similar.
  - There is a simple algorithmic description of the set.

  The initial image placed on the iterative function system (IFS) does not affect the final attractor. In fact, it is only the position and the orientation that determines what the final image will look like.

  In practice, affine transformations are rich enough and yield interesting set of attractors.

- DCT

  A discrete cosine transform (DCT) is a Fourier related transform which is similar to the discrete Fourier transform (DFT) but using only real numbers. 


Reference
1. Rafael C. Gonzalez, Richard E. Woods, Digital Image Processing, 2nd edition.
