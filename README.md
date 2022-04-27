# scancounter
Contador de producto con front-end (.NET Framework 4.6.1 - WinForm) y back-end (.NET 6 WPF)

Código para ARDBOX 20 I/O IndustrialShields
- Comunicación RS232 y alimentación 24VDC

```
#include <RS232.h>

// Lectura desde sensores de producto
boolean hitObject = false; // Valida si sensor de producto #1 ha leído
boolean hitObject2 = false; // Valida si sensor de producto #2 ha leído

// Lectura desde software 
int incomingByte = 0; // Obtiene caractér desde software (.NET)
boolean newData = false; // Valida si hay caracteres entrantes desde software.

int cont1 = 0; //Contador para sensor de producto #1
int cont2 = 0; //Contador para sensor de producto #2

int sensor1_Estado = 1; //Sensor inicia detenido
int sensor2_Estado = 1; //Sensor inicia detenido

void setup() {
  RS232.begin(9600);
}

void loop() {
  // Lee caracteres si hay disponible
  recvOneChar();
  showNewData();

  int val = digitalRead(I0_0); // Almacena lectura desde sensor #1 en variable
  int val2 = digitalRead(I0_1); // Almacena lectura desde sensor #2

  // Aumenta contador cuando detecta algo el sensor #1 e imprime
  // según estado del sensor (1: iniciado, 2: detenido)  
  switch(sensor1_Estado) {
    case 1:
      /*if((val == 0) && (hitObject == false)){ 
        RS232.print("A");
        hitObject = true;
      }else if((val == 1) && (hitObject == true)) {
        RS232.print("Z");
        hitObject = false;       
      }   */
      if(val==0){
        RS232.print("A");
      }else{
        RS232.print("Z");
      }
      delay(10);
      break;
    case 2:
      /*RS232.print("A:");
      RS232.print(cont1);
      RS232.println();*/
      delay(100);
      break;        
  }      

  // Aumenta contador cuando detecta algo el sensor #2 e imprime
  // según estado del sensor (1: iniciado, 2: detenido)  
  switch(sensor2_Estado) {
    case 1:
      /*if((val2 == 0) && (hitObject2 == false)){
        RS232.print("B");
        hitObject2 = true;
      }else if((val2 == 1) && (hitObject2 == true)) {
        RS232.print("Y");
        hitObject2 = false;    
      }   */
      if(val2==0){
        RS232.print("B");
      }else{
        RS232.print("Y");
      }
      delay(10);
      break;
    case 2:
      /*RS232.print("B:");
      RS232.print(cont2);
      RS232.println();*/
      delay(100);
      break;
  }  
}

// Almacena Serial.read en variable incomingByte cuando hay un caracter entrante
void recvOneChar() {
  /*if (Serial.available() > 0) {
    incomingByte = Serial.read();
    newData = true;
  }*/
  if (RS232.available() > 0) {
    incomingByte = RS232.read();
    newData = true;
  }
}

// Según el valor ASCII del caractér, hace algo
void showNewData() {
  if (newData == true) {
    switch (incomingByte) {
      case 97: //a, cambia estado de sensor1 a iniciado (1)  
        hitObject = true;
        sensor1_Estado = 1;
        delay(100); 
        break; 
      case 98: //b, cambia estado de sensor1 a pausado (2)        
        hitObject = true;
        sensor1_Estado = 2;
        delay(300);
        break;
      case 99: //c, reset sensor 1
        cont1 = 0;
        sensor1_Estado = 2;
        delay(100);
        break;
      case 100: //d, cambia estado de sensor2 a iniciado (1)   
        hitObject2 = true;     
        sensor2_Estado = 1;
        delay(100);
        break;
      case 101: //e, cambia estado de sensor2 a pausado (2) 
        hitObject2 = true;
        sensor2_Estado = 2;
        delay(100);
        break; 
      case 102: //f, reset sensor 2
        cont2 = 0;
        sensor2_Estado = 2;
        delay(100);
        break;   
    }
    
    newData = false;
  }
}
```
