#include "modbus.h"

AC1_Meter_typedef ac1;

#ifdef AC1_METER
M_STATUS GetVolt(unsigned long* Volt) {
	Volt[0] = ac1.Volt;
	return M_SUCCESS;
}
M_STATUS GetAMP(unsigned long* Amp) {
	Amp[0] = ac1.Amp;
	return M_SUCCESS;
}
M_STATUS GetPower(unsigned long* Power) {
	Power[0] = ac1.Power;
	return M_SUCCESS;
}
M_STATUS GetPower(unsigned long* En) {
	En[0] = ac1.Energy;
	return M_SUCCESS;
}
M_STATUS GetFrequncy(unsigned long* Fq) {
	Fq[0] = ac1.Frequency;
	return M_SUCCESS;
}
#endif

M_STATUS VerifiedPackage(unsigned char* dat, unsigned char len){
	unsigned byteCount;
	unsigned long _v,_i,_p,_e,_hm,_pf,_fq;
	unsigned short crc, crcI;
	if(dat[0] != ac1.NodeAdress) {return M_FAIL;}
	byteCount = dat[2];
	if(byteCount != 28) {return M_FAIL;}
	_v = (unsigned long)dat[3] << 24 |
			(unsigned long)dat[4] << 16 |
			(unsigned long)dat[5] << 8 |
			(unsigned long)dat[6];
	_i = (unsigned long)dat[7] << 24 |
				(unsigned long)dat[8] << 16 |
				(unsigned long)dat[9] << 8 |
				(unsigned long)dat[10];
	_p = (unsigned long)dat[11] << 24 |
				(unsigned long)dat[12] << 16 |
				(unsigned long)dat[13] << 8 |
				(unsigned long)dat[14];
	_e = (unsigned long)dat[15] << 24 |
					(unsigned long)dat[16] << 16 |
					(unsigned long)dat[17] << 8 |
					(unsigned long)dat[18];
	_hm = (unsigned long)dat[19] << 24 |
				(unsigned long)dat[20] << 16 |
				(unsigned long)dat[21] << 8 |
				(unsigned long)dat[22];
	_pf = (unsigned long)dat[23] << 24 |
					(unsigned long)dat[24] << 16 |
					(unsigned long)dat[25] << 8 |
					(unsigned long)dat[26];
	_fq = (unsigned long)dat[27] << 24 |
			(unsigned long)dat[28] << 16 |
			(unsigned long)dat[29] << 8 |
			(unsigned long)dat[30];
	CalCrc(dat, 31, &crc);
	if(crc != crcI) { return M_FAIL; }
	ac1.Volt = _v;
	ac1.Amp = _i;
	ac1.Power = _p;
	ac1.HourMeter = _hm;
	ac1.Energy = _e;
	ac1.Frequency = _fq;
	ac1.PowerFactor = _pf;
	return M_SUCCESS;
}
M_STATUS GenRead(ReadMsgStruct_typedef* obj, unsigned char* out, unsigned char* len) {
	unsigned short crc;
	out[0] = obj->Node;
	out[1] = obj->Fn;
	out[2] = (unsigned char)(obj->StartAddr >> 8);
	out[3] = (unsigned char)obj->StartAddr;
	out[4] = (unsigned char)(obj->Quantity >> 8);
	out[5] = (unsigned char)obj->Quantity;
	crc = CalCrc(out, 6, &crc);
	out[6] = (unsigned char)crc >> 8;
	out[7] = (unsigned char)crc;
	len[0] = 8;
	return M_SUCCESS;
}
void CalCrc(unsigned char* dat, unsigned char len, unsigned short* out) {
	 unsigned char cRCHI = 0xFF;
	 unsigned char cRCLO = 0xFF;
	 unsigned index;
	 unsigned dPtr = 0;
	 while (len--) {
		 index = cRCHI ^ dat[dPtr];
		 cRCHI = cRCLO ^ auchCRCHi[index];
		 cRCLO = auchCRCLo[index];
	 }
	 out[0] = (unsigned short)(cRCHI << 8) | (unsigned short)cRCLO;
}
