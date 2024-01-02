
#include <EEPROM.h>

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

int NuevoValor = 0; //valor asignable segun letra mayúscula de I0_0 - I0_12 O Q0_0 - Q0_6
int ModoPin = 0; //Definiendo por caracter  si es y = 1 entrada a si es z = 2 salida, 0 si no esta seteado 
int DirEdit = 0; //Identificador tabla en memoria EEPROM sobre variable a cambiar
//direcciones en  EEPROM
int DirEntr1 = 0; //entrada 1
int DirEntr2 = 1; // entrada 2
int DirSal1 = 2; // salida 1
//valores por default
int Entrada1 = (22); //I0_0
int Entrada2 = (23); //I0_1
int Salida1 = (36); //Q0_0
//valoresvoucher

void setup() {
  RS232.begin(9600); //inicia comunicación con un baud rate
  EEPROM.get(DirEntr1,Entrada1);
  EEPROM.get(DirEntr2,Entrada2);
  EEPROM.get(DirSal1,Salida1);
}

void loop() {
  // Lee caracteres si hay disponible
  recvOneChar();
  showNewData();
  
  int val = digitalRead(Entrada1); // Almacena lectura desde sensor #1 en variable
  int val2 = digitalRead(Entrada2); // Almacena lectura desde sensor #2
  // Aumenta contador cuando detecta algo el sensor #1 e imprime
  // según estado del sensor (1: iniciado, 2: detenido)  
 
    RS232.print(Entrada1);  
    delay(10);
 
  switch(sensor1_Estado) {
    case 1:
      if(val==0){
        RS232.print("A");
      }else{
        RS232.print("Z");
      }
      delay(10);
      break;
    case 2:
      delay(100);
      break;        
  }      

  // Aumenta contador cuando detecta algo el sensor #2 e imprime
  // según estado del sensor (1: iniciado, 2: detenido)  
  switch(sensor2_Estado) {
    case 1:
      if(val2==0){
        RS232.print("B");
      }else{
        RS232.print("Y");
      }
      delay(10);
      break;
    case 2:
      delay(100);
      break;
  }

 
}

// Almacena Serial.read en variable incomingByte cuando hay un caracter entrante
void recvOneChar() {
  if (RS232.available() > 0) {
    incomingByte = RS232.read();
    newData = true;
  }
}

//Almacena el valor entregado por programa en las memoria EEPROM y actualiza la salida de pines
void ConfirmEdit(){
  if(DirEdit > 0 && NuevoValor > 0){
    switch(DirEdit){
    case 1:
      Entrada1 = NuevoValor;
      break;
    case 2:
      Entrada2 = NuevoValor;
      break;  
    case 3:
      Salida1 = NuevoValor;
      break;
    }
    EEPROM.update(DirEdit, NuevoValor);
  }
  DirEdit = 0;
  NuevoValor = 0;
}

// Según el valor ASCII del caractér, hace algo
void showNewData() {
  if (newData == true) {
  
    switch (incomingByte) {
      case 97: //a, cambia estado de sensor1 a iniciado (1)  
        hitObject = true;
        sensor1_Estado = 1;
        break; 
      case 98: //b, cambia estado de sensor1 a pausado (2)        
        hitObject = true;
        sensor1_Estado = 2;
        break;
      case 99: //c, reset sensor 1
        cont1 = 0;
        sensor1_Estado = 2;
        break;
      case 100: //d, cambia estado de sensor2 a iniciado (1)   
        hitObject2 = true;     
        sensor2_Estado = 1;
        break;
      case 101: //e, cambia estado de sensor2 a pausado (2) 
        hitObject2 = true;
        sensor2_Estado = 2;
        break; 
      case 102: //f, reset sensor 2
        cont2 = 0;
        sensor2_Estado = 2;
        break;   
        //CAMBIO DE PINES
        //letras minusculas con para controles , letras mayusculas son para pines
      case 120: // x , fila eprom 1 . entrada 1
        DirEdit = 1;
        break;
      case 121: // y , filaeprom 2 - entrada 2
        DirEdit = 2;
        break;
      case 122: // z , fila eeprom 3 - salida1
        DirEdit = 3;
        break; 
      case 65: //A
        NuevoValor = I0_0;
        break;
      case 66: //B
        NuevoValor = I0_1;
        break;
      case 67: //C
        NuevoValor = I0_2;
        break;
      case 68: //D
        NuevoValor = I0_3;
        break;
      case 69: //F
        NuevoValor = I0_4;
        break;
      case 70: //G
        NuevoValor = I0_5;
        break;
      case 71: //H
        NuevoValor = I0_6;
        break;
      case 72: //I
        NuevoValor = I0_7;
        break;
      case 73: //J
        NuevoValor = I0_8;
        break;
      case 74: //K
        NuevoValor = I0_9;
        break;
      case 75: //L
        NuevoValor = I0_10;
          break;
      case 76: //M
        NuevoValor = I0_11;
        break;
      case 77: //N
        NuevoValor = I0_12;
        break;
      case 78: //O
        NuevoValor = Q0_0;
        break;
      case 79: //P
        NuevoValor = Q0_1;
        break;
      case 80: //Q
        NuevoValor = Q0_2;
        break;
      case 81: //R
        NuevoValor = Q0_3;
        break;
      case 82: //S
        NuevoValor = Q0_4;
        break;
      case 83: //T
        NuevoValor = Q0_5;
        break;
      case 84: //U
        NuevoValor = Q0_6;
        break;
      case 48: // 0 Confirmacion para cambiar
        ConfirmEdit();
        break;
    }
    newData = false;
  }
}