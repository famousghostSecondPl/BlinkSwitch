# Blink Switch

A simple FPS prototype featuring split-screen gameplay and multiple real-time post-processing effects.  
The project focuses on experimenting with screen-space rendering techniques and visual stylization.

---

## 📌 Overview

Blink Switch is a small experimental project built in Unity, designed to explore
real-time post-processing techniques and rendering pipelines in an interactive FPS environment.

The project combines gameplay with visual experimentation, allowing runtime switching
between multiple stylized rendering modes.

---

## 🚀 Core Features

- Multiple post-processing modes with runtime switching  
- Cartoon-style rendering using outline detection and dithering  
- Sketch effect based on **Difference of Gaussians (DoG)** and edge detection  
- Old TV screen effect using UV distortion and sinusoidal noise functions  
- Split-screen gameplay using Unity Input System with full gamepad support  
- Custom lightweight **G-Buffer** to improve outline stability on close geometry  
- Basic **Temporal Anti-Aliasing (TAA)** implementation  

---

## 🧠 Technical Highlights

### Post-Processing Pipeline
Custom screen-space effects implemented using shaders, including:
- Edge detection using depth and normal buffers  
- Stylized shading via luminance and dithering  
- UV-space distortions for retro effects  

### Custom G-Buffer
A lightweight G-Buffer was introduced to:
- Improve consistency of outline rendering  
- Handle edge cases with overlapping and close geometry  

### Temporal Anti-Aliasing (TAA)
A basic TAA solution was implemented to:
- Reduce flickering and aliasing artifacts  
- Stabilize image across frames  

### Split-Screen System
- Built using Unity's modern Input System  
- Supports multiple players with gamepad input  
- Independent camera rendering per viewport  

---

## 🎥 Video Demonstrations

### Split-Screen Effect
Demonstrates multi-player rendering and camera separation.
[![Split-screen demo]](media/Blink-switch-split-screen.mp4)

### Post-Processing Switching
Shows runtime switching between different visual styles.
[![Split-screen demo]](media/Blink-switch-effects.mp4)

### Temporal Anti-Aliasing (TAA)
Illustrates reduction of flickering and improved visual stability.
[![Split-screen demo]](media/TAA.mp4)

---

## 🛠️ Technologies

- **Engine:** Unity (Built-in Render Pipeline)  
- **Language:** C#  
- **Graphics:** Custom shaders (HLSL)  
- **Unity Version:** 6000.1.13f1  

---

## 📚 What I Learned

- Implementation of screen-space post-processing effects  
- Managing rendering consistency with custom buffers  
- Trade-offs between visual quality and performance  
- Handling multi-camera rendering for split-screen systems  
- Basics of temporal techniques like TAA  

---

## 📌 Notes

This project is focused on experimentation and learning rather than production-ready systems.
Some implementations (e.g., TAA, G-Buffer) are simplified versions intended for exploration.

---

## 👤 Author

Marcin Czekaj