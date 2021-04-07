#ifndef CAUSTIC
#define CAUSTIC


void MaskCaustic_float(float3 WorldPos, float WaterHeight, float3 ColourF, float3 ColourT, out float3 Out){ 
    if (WorldPos.y < WaterHeight) {   
        Out = ColourT;
    } else {
        Out = ColourF;
    }
}
#endif //CAUSTIC